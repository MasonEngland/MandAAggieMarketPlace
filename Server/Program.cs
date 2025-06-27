/*
    * Manda Marketplace copyright (c) 2023-2025
    * Created by Mason England
    * 
    * The structure of the server uses the Model View Controller (MVC) pattern.
    * Then a service layer is added to handle business logic to handel database queries and data manipulation.
    * Built on React, ASP.NET Core, MySQL, and Entity Framework Core.
    *
*/

using Server.Context;
using Microsoft.EntityFrameworkCore;
using Server.Middleware;
using Server.Services;

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
        builder.Services.AddCustomServices();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 

        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddDbContext<DatabaseContext>(options => 
        {
            options.UseMySQL(builder.Configuration.GetConnectionString("Default")!);
        });

        var app = builder.Build();

        // //! remove during deployment
        // app.UseCors(builder => {
        //     builder.WithOrigins("http://localhost:5173");
        //     builder.AllowAnyMethod();
        //     builder.AllowAnyHeader();
        // });

        app.UseAuthorization();
        

        app.UseStaticFiles();

        app.UseAuthToken();

        app.MapControllers();


        app.Run();
    }
}