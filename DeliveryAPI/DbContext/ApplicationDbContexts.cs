using DeliveryAPI.Models.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI;

public class ApplicationDbContexts : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<DishInCart?> DishBaskets { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public ApplicationDbContexts(DbContextOptions<ApplicationDbContexts> options):
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
    }
}