using Server.Context;
using Microsoft.EntityFrameworkCore;
using Server.Middleware;

namespace Server;

public class Program 
{
    public static void Main(string[] args) 
    {
        DotEnv.Config("./.env");
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddTransient<AuthToken>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 

        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddDbContext<DatabaseContext>(options => 
        {
            options.UseMySQL(builder.Configuration.GetConnectionString("Default")!);
        });

        var app = builder.Build();

        //TODO: remove during deployment
        app.UseCors(builder => {
            builder.WithOrigins("http://localhost:5173");
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
        });

        app.UseAuthorization();
        

        app.UseStaticFiles();

        app.UseAuthToken();

        app.MapControllers();


        app.Run();
    }
}





