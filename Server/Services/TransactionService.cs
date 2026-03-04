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
    private readonly IStripeService _stripeService;

    public TransactionService(DatabaseContext db, ICommerceService commerceService, IStripeService stripeService)
    {
        _db = db;
        _commerceService = commerceService;
        _stripeService = stripeService;
    }

    public async Task<string?> CreateCheckoutSession(CartItem[] items, string address, string applicationUrl)
    {
        // pass through for interface reasons. 
        // this was to move stripe logic to another service for readability
        return await _stripeService.CreateCheckoutSession(items, address, applicationUrl);
    }




    public async Task<bool> HandleCheckoutStatus(string sessionId, string accountId, string address)
    {
        try
        {
            Session session = _stripeService.GetSession(sessionId);
            Models.Account account = GetAccount(accountId);

            if (!IsPaymentSuccessful(session))
            {
                return false;
            }

            string[] itemIds = GetItemIds(session);
            if (itemIds == null || itemIds.Length <= 0)
            {
                return false;
            }

            Item[] dbItems = await GetDatabaseItems(itemIds);
            if (dbItems.Length <= 0)
            {
                return false;
            }

            foreach (Item dbItem in dbItems)
            {
                if (!await ProcessItemPurchase(dbItem, account, address, session, sessionId))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCheckoutStatus: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            return false;
        }
    }

    public async Task<string?> GetSecret(string sessionId)
    {
        return await _stripeService.GetSecret(sessionId);
    }

    private async Task<bool> ProcessPurchase(Models.Account account, Item item, string address, Session session, string sessionId) 
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            Item[] dbItem = await _db.CurrentStock.Where(i => i.Id == item.Id).ToArrayAsync();
            
            (bool success, Item? dbItem) result = await _commerceService.Purchase(account, item, address);

            if (!result.success) throw new Exception($"Purchase failed for item {item.Name}");

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

    private Models.Account GetAccount(string accountId)
    {
        return _db.accounts.First(a => a.Id.ToString() == accountId);
    }

    private bool IsPaymentSuccessful(Session session)
    {
        return session.PaymentStatus == "paid";
    }

    private string[] GetItemIds(Session session)
    {
        return session.Metadata["itemIds"].Split(',');
    }

    private async Task<Item[]> GetDatabaseItems(string[] itemIds)
    {
        try
        {
            Item[] dbItems = await _db.CurrentStock.Where(i => itemIds.Contains(i.Id.ToString())).ToArrayAsync();
            return dbItems;


        } catch (Exception err)
        {
            Console.WriteLine("Error fetching database items: " + err.Message);
            Console.WriteLine(err.StackTrace);
            return new Item[0];
        }
    }



    private async Task<bool> ProcessItemPurchase(Item dbItem, Models.Account account, string address, Session session, string sessionId)
    {
        // fetch line item for this db item
        LineItem? lineItem = session.LineItems.Data.First(li => li.Metadata["itemId"] == dbItem.Id.ToString());
        long? quantity = lineItem.Quantity;

        if (quantity == null || quantity <= 0)
        {
            return false;
        }
        
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

        return await ProcessPurchase(account, purchaseItem, address, session, sessionId);
    }
}