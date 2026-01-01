using Server.Services;


public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        // Register custom services here
        services.AddScoped<ICommerceService, CommerceService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<ITransactionService, TransactionService>();
    }
}