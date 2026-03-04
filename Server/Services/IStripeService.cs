using Server.Models;
using Stripe.Checkout;

public interface IStripeService
{
    Task<string?> CreateCheckoutSession(CartItem[] items, string address, string applicationUrl);
    Task<string?> GetSecret(string sessionId);

    Session GetSession(string sessionId);

}
