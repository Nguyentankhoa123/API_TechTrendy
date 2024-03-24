using API.Model.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class StoreContext : IdentityDbContext<ApplicationUser>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Tablet> Tablets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<UserAddress> UserAddresses { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        public DbSet<DiscountUser> DiscountsUser { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
