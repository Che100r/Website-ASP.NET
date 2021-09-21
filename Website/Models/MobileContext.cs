using Microsoft.EntityFrameworkCore;

namespace Website.Models
{
    public class MobileContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public MobileContext(DbContextOptions<MobileContext> options)
            : base(options)
        {
            // Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
