using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Context;

public class DatabaseContext : DbContext
{
    public DbSet<Account> accounts {get; set;}

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}
