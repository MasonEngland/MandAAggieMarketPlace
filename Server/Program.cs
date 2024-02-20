using Server.Context;
using Microsoft.EntityFrameworkCore;

namespace Server;

public class Program 
{
    public static void Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        

        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddDbContext<DatabaseContext>(options => 
        {
            options.UseMySQL(builder.Configuration.GetConnectionString("Default")!);
        });

        var app = builder.Build();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}





