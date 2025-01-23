using Microsoft.EntityFrameworkCore;
using apiApp.Models;

namespace apiApp.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    public DbSet<Car> Cars { get; set; }
    public DbSet<Person> People { get; set; }
    
}