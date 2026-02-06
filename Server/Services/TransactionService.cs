using Microsoft.EntityFrameworkCore;
using Server.Context;
using Server.Models;
using Stripe.Checkout;
using Stripe;

namespace Server.Services;

public class TransactionService : ITransactionService
{
    private readonly DatabaseContext _db;
    private readonly ICommerceService _commerceService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TransactionService(DatabaseContext db, ICommerceService commerceService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _commerceService = commerceService;
        _httpContextAccessor = httpContextAccessor;

    }

    public async Task<string?> CreateCheckoutSession(CartItem[] items, string address)
    {
        string applicationUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
        List<SessionLineItemOptions> lineItems = new List<SessionLineItemOptions>();


        Item[] dbItems = new Item[items.Length];
        string[] itemIds = new string[items.Length];

        var transaction = await _db.Database.BeginTransactionAsync();

        for (int i = 0; i < items.Length; i++)
        {
            Item? dbItem = await _db.CurrentStock.FirstOrDefaultAsync(h => h.Id == items[i].OrderItem.Id);
            if (dbItem == null)
            {
                return null;
            }
            dbItem.Stock = items[i].Amount;
            dbItems[i] = dbItem;
            itemIds[i] = dbItem.Id.ToString();
        }

        foreach (Item item in dbItems)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name,
                        Description = item.Description,
                    },
                },
                Quantity = item.Stock,
            });
        }
        
        
        // create the stripe checkout session
        var sessionoptions = new SessionCreateOptions
        {
            LineItems = lineItems,
            Mode = "payment",
            UiMode = "embedded",
            ReturnUrl = $"{applicationUrl}/status?session_id={{CHECKOUT_SESSION_ID}}&address={address}",
            Metadata = new Dictionary<string, string>
            {
                { "itemIds", string.Join(",", itemIds) },
            },

        };
        var sessionService = new SessionService();
        Session session = sessionService.Create(sessionoptions);

        await transaction.RollbackAsync();
        return session.Id;
    }

    public async Task<bool> HandleCheckoutStatus(string sessionId, string accountId, string address)
    {
        try
        {
            
        
            SessionService sessionService = new SessionService();
            var options = new SessionGetOptions
            {
                Expand = new List<string> { "line_items" },
            };
            Session session = sessionService.Get(sessionId, options);
            Models.Account account = _db.accounts.First(a => a.Id.ToString() == accountId);

            if (session.PaymentStatus != "paid")
            {
                return false;
            }

            string[] itemIds = session.Metadata["itemIds"].Split(',');
            if (itemIds.Length <= 0 || itemIds == null) return false;


            


            Item[] dbItems = new Item[itemIds.Length];
            for (int i = 0; i < itemIds.Length; i++)
            {
                Item? dbItem = await _db.CurrentStock.FirstOrDefaultAsync(h => h.Id.ToString() == itemIds[i]);
                if (dbItem == null)
                {
                    return false;
                }
                dbItems[i] = dbItem;
            }

            foreach (Item dbItem in dbItems)
            {
                long? quantity = session.LineItems.Data[0].Quantity;
                if (quantity == null) return false;
                Item purchaseItem = new Item
                {
                    Id = dbItem.Id,
                    Name = dbItem.Name,
                    Description = dbItem.Description,
                    Price = dbItem.Price,
                    Stock = (int)quantity,
                    Date = dbItem.Date,
                    ImageLink = dbItem.ImageLink,
                    Category = dbItem.Category,
                };
                bool purchaseResult = await ProcessPurchase(account, purchaseItem, address, session, sessionId);
                if (!purchaseResult)
                {
                    return false;
                }

            }

            
            return true;
        } catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCheckoutStatus: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }

    public async Task<string?> GetSecret(string sessionId)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(sessionId);
        return session.ClientSecret;
    }

    private async Task<bool> ProcessPurchase(Models.Account account, Item item, string address, Session session, string sessionId) 
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            
            
            (bool success, Item? item) result = await _commerceService.Purchase(account, item, address);

            if (!result.success) return false;

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            
            // Issue refund since payment was processed but DB update failed
            try
            {
                var refundService = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = session.PaymentIntentId,
                };
                Refund refund = await refundService.CreateAsync(refundOptions);
                // check if refund was successful
                if (refund.Status != "succeeded")
                {
                    Console.WriteLine($"Refund failed for session {sessionId}: Status {refund.Status}");
                    return false;
                }
            }
            catch (Exception refundEx)
            {
                // Log refund failure - may need manual intervention
                Console.WriteLine($"Refund failed for session {sessionId}: {refundEx.Message}");
                //TODO: possibly send alert to admin for manual intervention
            }
            
            return false;
        }
    }
}