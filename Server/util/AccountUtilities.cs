using Server.Models;
using System.Text.Json;
using JWT.Builder;
using JWT.Algorithms;

namespace Server.Util;

public static class AccountUtilities
{
    public static Account? GetAccount(HttpContext context)
    {
        //
        // gets the token data from the http contex and deserializes it 
        // returns null or the account object depending on the success of the deserialization
        //
        string? data = Convert.ToString(context.Items["tokenData"]);
        if (data == null)
        {
            return null;
        }

        Account? account = JsonSerializer.Deserialize<Account>(data!);
        if (account == null)
        {
            return null;
        }
        return account;
    }

    public static string? MakeToken(Account account)
    {
        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;
        string token = JwtBuilder
            .Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(secret)
            .AddClaim("Id", account.Id)
            .AddClaim("Email", account.Email)
            .AddClaim("FirstName", account.FirstName)
            .AddClaim("LastName", account.LastName)
            .AddClaim("Admin", account.Admin)
            .AddClaim("Balance", account.Balance)
            .Encode();

        return token;
    }

}