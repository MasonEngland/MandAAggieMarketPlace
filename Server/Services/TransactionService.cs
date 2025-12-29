using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Server.Context;
using Server.Models;
using Stripe.Checkout;

public class TransactionService : ITransactionService
{
    private readonly DatabaseContext _db;

    public TransactionService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<object?> CreateCheckoutSession(double amount, Item[] items)
    {
        string applicationUrl = Environment.GetEnvironmentVariable("APPLICATION_URL")!;
        List<SessionLineItemOptions> lineItems = new List<SessionLineItemOptions>();


        for (int i = 0; i< items.Length; i++)
        {
            Item item = items[i];
            Item dbItem = await _db.CurrentStock.FirstAsync(h => h.Id == item.Id);

            if (dbItem.Stock < item.Stock || item.Stock <= 0)
            {
                return null;
            }

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

        var sessionoptions = new SessionCreateOptions
        {
            LineItems = lineItems,
            Mode = "payment",
            UiMode = "embedded",
            ReturnUrl = $"{applicationUrl}/checkout-status?session_id={{CHECKOUT_SESSION_ID}}" // enter url to handle checkout status

        };
        var sessionService = new SessionService();
        Session session = sessionService.Create(sessionoptions);
        return session.ClientSecret;

    }

    public bool HandleCheckoutStatus(string sessionId)
    {
        throw new NotImplementedException();
    }
}