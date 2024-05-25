using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Context;

public class DatabaseContext : DbContext
{
    public DbSet<Account> accounts {get; set;}
    public DbSet<Item> CurrentStock { get; set; }
    public DbSet<Order> OrderQueue { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}
}
