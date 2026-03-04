using Stripe.Checkout;
using Server.Models;
using Server.Context;
using Microsoft.EntityFrameworkCore;


namespace Server.Services;

public class StripeService : IStripeService
{
    private readonly DatabaseContext _db;

    public StripeService(DatabaseContext db)
    {
        _db = db;
    }
    public async Task<string?> CreateCheckoutSession(CartItem[] items, string address, string applicationUrl)
    {
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

        // create a list of line items for the checkout session
        // each line items contains the item id for future reference
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
                        Metadata = new Dictionary<string, string>
                        {
                            { "itemId", item.Id.ToString() },
                        },
                    },
                },
                Quantity = item.Stock,
            });
        }
        
        
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

    public async Task<string?> GetSecret(string sessionId)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(sessionId);
        return session.ClientSecret;
    }

    public Session GetSession(string sessionId)
    {
        SessionService sessionService = new SessionService();
        var options = new SessionGetOptions
        {
            Expand = new List<string> { "line_items" },
        };
        return sessionService.Get(sessionId, options);
    }

}