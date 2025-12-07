using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FashionStore.Models.Entities;

namespace FashionStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            // Disable automatic migrations
            Database.SetInitializer<ApplicationDbContext>(new DatabaseInitializer());
            
            // Disable proxy creation for better performance
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove pluralization
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Configure decimal precision
            ConfigureDecimalPrecision(modelBuilder);

            // Configure relationships
            ConfigureRelationships(modelBuilder);

            // Configure indexes
            ConfigureIndexes(modelBuilder);
        }

        private void ConfigureDecimalPrecision(DbModelBuilder modelBuilder)
        {
            // Product decimal properties
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasPrecision(18, 2);

            // Order decimal properties
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            // OrderDetail decimal properties
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.SubTotal)
                .HasPrecision(18, 2);
        }

        private void ConfigureRelationships(DbModelBuilder modelBuilder)
        {
            // User - Role relationship
            modelBuilder.Entity<User>()
                .HasRequired(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .WillCascadeOnDelete(false);

            // Product - Category relationship
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .WillCascadeOnDelete(false);

            // Order - User relationship
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            // OrderDetail - Order relationship (CASCADE DELETE)
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .WillCascadeOnDelete(true);

            // OrderDetail - Product relationship
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .WillCascadeOnDelete(false);

            // Cart - User relationship (One-to-One with CASCADE DELETE)
            // Using HasRequired().WithMany() with unique constraint to enforce one-to-one
            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(true);
            
            // Ensure UserId has unique index for one-to-one relationship
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            // CartItem - Cart relationship (CASCADE DELETE)
            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .WillCascadeOnDelete(true);

            // CartItem - Product relationship
            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .WillCascadeOnDelete(false);

            // Unique constraint: One product per cart
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();
        }

        private void ConfigureIndexes(DbModelBuilder modelBuilder)
        {
            // Product indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasName("IX_Products_CategoryId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Featured)
                .HasName("IX_Products_Featured");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.IsActive)
                .HasName("IX_Products_IsActive");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CreatedDate)
                .HasName("IX_Products_CreatedDate");

            // Order indexes
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId)
                .HasName("IX_Orders_UserId");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate)
                .HasName("IX_Orders_OrderDate");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status)
                .HasName("IX_Orders_Status");
        }
    }
}

