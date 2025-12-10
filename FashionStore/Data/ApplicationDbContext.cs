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
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        
        // Payment & Shipping
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Return> Returns { get; set; }
        
        // Inventory
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        
        // Promotions & Coupons
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        
        // Permissions & Audit
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        // Customer Management
        public DbSet<Address> Addresses { get; set; }
        
        // Media & Collections
        public DbSet<Media> Media { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<ProductCollection> ProductCollections { get; set; }
        
        // Reviews & Comments
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<ReviewHelpful> ReviewHelpfuls { get; set; }
        public DbSet<ReviewReport> ReviewReports { get; set; }

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
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.CostPrice)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Order decimal properties
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // OrderDetail decimal properties
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.SubTotal)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Order additional decimal properties
            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Payment decimal properties
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Refund decimal properties
            modelBuilder.Entity<Refund>()
                .Property(r => r.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Shipment decimal properties
            modelBuilder.Entity<Shipment>()
                .Property(s => s.ShippingFee)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Promotion decimal properties
            modelBuilder.Entity<Promotion>()
                .Property(pr => pr.DiscountPercentage)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Promotion>()
                .Property(pr => pr.DiscountAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Promotion>()
                .Property(pr => pr.MinimumOrderAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Promotion>()
                .Property(pr => pr.MaximumDiscountAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // Coupon decimal properties
            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountPercentage)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.MinimumOrderAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.MaximumDiscountAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // CouponUsage decimal properties
            modelBuilder.Entity<CouponUsage>()
                .Property(cu => cu.DiscountAmount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // ProductVariant decimal properties
            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            // User decimal properties
            modelBuilder.Entity<User>()
                .Property(u => u.TotalOrderValue)
                .HasColumnType("decimal")
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

            // ProductVariant - Product relationship
            modelBuilder.Entity<ProductVariant>()
                .HasRequired(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .WillCascadeOnDelete(true);

            // Payment - Order relationship
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .WillCascadeOnDelete(false);

            // Refund - Order relationship
            modelBuilder.Entity<Refund>()
                .HasRequired(r => r.Order)
                .WithMany(o => o.Refunds)
                .HasForeignKey(r => r.OrderId)
                .WillCascadeOnDelete(false);

            // Refund - Payment relationship
            modelBuilder.Entity<Refund>()
                .HasOptional(r => r.Payment)
                .WithMany()
                .HasForeignKey(r => r.PaymentId)
                .WillCascadeOnDelete(false);

            // Refund - User (ProcessedBy) relationship
            modelBuilder.Entity<Refund>()
                .HasOptional(r => r.ProcessedBy)
                .WithMany()
                .HasForeignKey(r => r.ProcessedByUserId)
                .WillCascadeOnDelete(false);

            // Shipment - Order relationship
            modelBuilder.Entity<Shipment>()
                .HasRequired(s => s.Order)
                .WithMany(o => o.Shipments)
                .HasForeignKey(s => s.OrderId)
                .WillCascadeOnDelete(false);

            // Return - Order relationship
            modelBuilder.Entity<Return>()
                .HasRequired(r => r.Order)
                .WithMany(o => o.Returns)
                .HasForeignKey(r => r.OrderId)
                .WillCascadeOnDelete(false);

            // Return - User (ProcessedBy) relationship
            modelBuilder.Entity<Return>()
                .HasOptional(r => r.ProcessedBy)
                .WithMany()
                .HasForeignKey(r => r.ProcessedByUserId)
                .WillCascadeOnDelete(false);

            // InventoryTransaction - Product relationship
            modelBuilder.Entity<InventoryTransaction>()
                .HasRequired(it => it.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.ProductId)
                .WillCascadeOnDelete(false);

            // InventoryTransaction - ProductVariant relationship
            modelBuilder.Entity<InventoryTransaction>()
                .HasOptional(it => it.ProductVariant)
                .WithMany()
                .HasForeignKey(it => it.ProductVariantId)
                .WillCascadeOnDelete(false);

            // InventoryTransaction - Order relationship
            modelBuilder.Entity<InventoryTransaction>()
                .HasOptional(it => it.Order)
                .WithMany(o => o.InventoryTransactions)
                .HasForeignKey(it => it.OrderId)
                .WillCascadeOnDelete(false);

            // InventoryTransaction - User (CreatedBy) relationship
            modelBuilder.Entity<InventoryTransaction>()
                .HasOptional(it => it.CreatedBy)
                .WithMany()
                .HasForeignKey(it => it.CreatedByUserId)
                .WillCascadeOnDelete(false);

            // CouponUsage - Coupon relationship
            modelBuilder.Entity<CouponUsage>()
                .HasRequired(cu => cu.Coupon)
                .WithMany()
                .HasForeignKey(cu => cu.CouponId)
                .WillCascadeOnDelete(false);

            // CouponUsage - Order relationship
            modelBuilder.Entity<CouponUsage>()
                .HasRequired(cu => cu.Order)
                .WithMany(o => o.CouponUsages)
                .HasForeignKey(cu => cu.OrderId)
                .WillCascadeOnDelete(false);

            // CouponUsage - User relationship
            modelBuilder.Entity<CouponUsage>()
                .HasRequired(cu => cu.User)
                .WithMany(u => u.CouponUsages)
                .HasForeignKey(cu => cu.UserId)
                .WillCascadeOnDelete(false);

            // RolePermission - Role relationship
            modelBuilder.Entity<RolePermission>()
                .HasRequired(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .WillCascadeOnDelete(true);

            // RolePermission - Permission relationship
            modelBuilder.Entity<RolePermission>()
                .HasRequired(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .WillCascadeOnDelete(true);

            // AuditLog - User relationship
            modelBuilder.Entity<AuditLog>()
                .HasOptional(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .WillCascadeOnDelete(false);

            // Address - User relationship
            modelBuilder.Entity<Address>()
                .HasRequired(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(true);

            // Media - Product relationship
            modelBuilder.Entity<Media>()
                .HasOptional(m => m.Product)
                .WithMany(p => p.Media)
                .HasForeignKey(m => m.ProductId)
                .WillCascadeOnDelete(true);

            // Media - ProductVariant relationship
            // Không dùng cascade delete để tránh multiple cascade paths (Product -> Media và Product -> ProductVariant -> Media)
            modelBuilder.Entity<Media>()
                .HasOptional(m => m.ProductVariant)
                .WithMany()
                .HasForeignKey(m => m.ProductVariantId)
                .WillCascadeOnDelete(false);

            // ProductCollection - Product relationship
            modelBuilder.Entity<ProductCollection>()
                .HasRequired(pc => pc.Product)
                .WithMany(p => p.ProductCollections)
                .HasForeignKey(pc => pc.ProductId)
                .WillCascadeOnDelete(true);

            // ProductCollection - Collection relationship
            modelBuilder.Entity<ProductCollection>()
                .HasRequired(pc => pc.Collection)
                .WithMany(c => c.ProductCollections)
                .HasForeignKey(pc => pc.CollectionId)
                .WillCascadeOnDelete(true);

            // Category - ParentCategory relationship (self-referencing)
            modelBuilder.Entity<Category>()
                .HasOptional(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
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

            // ProductReview indexes
            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => pr.ProductId)
                .HasName("IX_ProductReviews_ProductId");

            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => pr.UserId)
                .HasName("IX_ProductReviews_UserId");

            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => pr.Status)
                .HasName("IX_ProductReviews_Status");

            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => pr.Rating)
                .HasName("IX_ProductReviews_Rating");

            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => new { pr.ProductId, pr.UserId })
                .HasName("IX_ProductReviews_ProductId_UserId");

            // ReviewComment indexes
            modelBuilder.Entity<ReviewComment>()
                .HasIndex(rc => rc.ProductReviewId)
                .HasName("IX_ReviewComments_ProductReviewId");

            modelBuilder.Entity<ReviewComment>()
                .HasIndex(rc => rc.Status)
                .HasName("IX_ReviewComments_Status");

            // ReviewReport indexes
            modelBuilder.Entity<ReviewReport>()
                .HasIndex(rr => rr.Status)
                .HasName("IX_ReviewReports_Status");

            modelBuilder.Entity<ReviewReport>()
                .HasIndex(rr => rr.ReportType)
                .HasName("IX_ReviewReports_ReportType");

            // Payment indexes
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.OrderId)
                .HasName("IX_Payments_OrderId");

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.Status)
                .HasName("IX_Payments_Status");

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.TransactionId)
                .HasName("IX_Payments_TransactionId");

            // Coupon indexes
            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .HasName("IX_Coupons_Code");

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.IsActive)
                .HasName("IX_Coupons_IsActive");

            // InventoryTransaction indexes
            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => it.ProductId)
                .HasName("IX_InventoryTransactions_ProductId");

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => it.TransactionType)
                .HasName("IX_InventoryTransactions_TransactionType");

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(it => it.CreatedDate)
                .HasName("IX_InventoryTransactions_CreatedDate");

            // AuditLog indexes
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.TableName)
                .HasName("IX_AuditLogs_TableName");

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => new { al.TableName, al.RecordId })
                .HasName("IX_AuditLogs_TableName_RecordId");

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.CreatedDate)
                .HasName("IX_AuditLogs_CreatedDate");

            // Address indexes
            modelBuilder.Entity<Address>()
                .HasIndex(a => a.UserId)
                .HasName("IX_Addresses_UserId");

            // Media indexes
            modelBuilder.Entity<Media>()
                .HasIndex(m => m.ProductId)
                .HasName("IX_Media_ProductId");

            // ProductCollection unique constraint
            modelBuilder.Entity<ProductCollection>()
                .HasIndex(pc => new { pc.ProductId, pc.CollectionId })
                .IsUnique()
                .HasName("IX_ProductCollections_ProductId_CollectionId");

            // Category Slug index (không unique ở đây để tránh lỗi nhiều NULL)
            // Unique constraint sẽ được đảm bảo ở application level hoặc bằng filtered index trong SQL
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .HasName("IX_Categories_Slug");

            // Product Slug và SKU indexes (không unique để tránh lỗi nhiều NULL)
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .HasName("IX_Products_Slug");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .HasName("IX_Products_SKU");

            // Collection Slug index
            modelBuilder.Entity<Collection>()
                .HasIndex(c => c.Slug)
                .HasName("IX_Collections_Slug");

            // ProductReview - Product relationship
            modelBuilder.Entity<ProductReview>()
                .HasRequired(pr => pr.Product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductId)
                .WillCascadeOnDelete(false);

            // ProductReview - User relationship
            modelBuilder.Entity<ProductReview>()
                .HasRequired(pr => pr.User)
                .WithMany(u => u.ProductReviews)
                .HasForeignKey(pr => pr.UserId)
                .WillCascadeOnDelete(false);

            // ProductReview - Order relationship
            modelBuilder.Entity<ProductReview>()
                .HasOptional(pr => pr.Order)
                .WithMany(o => o.ProductReviews)
                .HasForeignKey(pr => pr.OrderId)
                .WillCascadeOnDelete(false);

            // ProductReview - User (ReviewedBy) relationship
            modelBuilder.Entity<ProductReview>()
                .HasOptional(pr => pr.ReviewedBy)
                .WithMany()
                .HasForeignKey(pr => pr.ReviewedByUserId)
                .WillCascadeOnDelete(false);

            // ReviewImage - ProductReview relationship
            modelBuilder.Entity<ReviewImage>()
                .HasRequired(ri => ri.ProductReview)
                .WithMany(pr => pr.ReviewImages)
                .HasForeignKey(ri => ri.ProductReviewId)
                .WillCascadeOnDelete(true);

            // ReviewComment - ProductReview relationship
            modelBuilder.Entity<ReviewComment>()
                .HasRequired(rc => rc.ProductReview)
                .WithMany(pr => pr.ReviewComments)
                .HasForeignKey(rc => rc.ProductReviewId)
                .WillCascadeOnDelete(true);

            // ReviewComment - User relationship
            modelBuilder.Entity<ReviewComment>()
                .HasRequired(rc => rc.User)
                .WithMany(u => u.ReviewComments)
                .HasForeignKey(rc => rc.UserId)
                .WillCascadeOnDelete(false);

            // ReviewComment - ParentComment relationship (self-referencing)
            modelBuilder.Entity<ReviewComment>()
                .HasOptional(rc => rc.ParentComment)
                .WithMany(rc => rc.ChildComments)
                .HasForeignKey(rc => rc.ParentCommentId)
                .WillCascadeOnDelete(false);

            // ReviewHelpful - ProductReview relationship
            modelBuilder.Entity<ReviewHelpful>()
                .HasRequired(rh => rh.ProductReview)
                .WithMany(pr => pr.ReviewHelpfuls)
                .HasForeignKey(rh => rh.ProductReviewId)
                .WillCascadeOnDelete(true);

            // ReviewHelpful - User relationship
            modelBuilder.Entity<ReviewHelpful>()
                .HasRequired(rh => rh.User)
                .WithMany(u => u.ReviewHelpfuls)
                .HasForeignKey(rh => rh.UserId)
                .WillCascadeOnDelete(false);

            // ReviewHelpful unique constraint: Mỗi user chỉ vote một lần cho mỗi review
            modelBuilder.Entity<ReviewHelpful>()
                .HasIndex(rh => new { rh.ProductReviewId, rh.UserId })
                .IsUnique()
                .HasName("IX_ReviewHelpfuls_ProductReviewId_UserId");

            // ReviewReport - ProductReview relationship
            modelBuilder.Entity<ReviewReport>()
                .HasOptional(rr => rr.ProductReview)
                .WithMany(pr => pr.ReviewReports)
                .HasForeignKey(rr => rr.ProductReviewId)
                .WillCascadeOnDelete(false);

            // ReviewReport - ReviewComment relationship
            modelBuilder.Entity<ReviewReport>()
                .HasOptional(rr => rr.ReviewComment)
                .WithMany()
                .HasForeignKey(rr => rr.ReviewCommentId)
                .WillCascadeOnDelete(false);

            // ReviewReport - User relationship
            modelBuilder.Entity<ReviewReport>()
                .HasRequired(rr => rr.User)
                .WithMany(u => u.ReviewReports)
                .HasForeignKey(rr => rr.UserId)
                .WillCascadeOnDelete(false);

            // ReviewReport - User (ReviewedBy) relationship
            modelBuilder.Entity<ReviewReport>()
                .HasOptional(rr => rr.ReviewedBy)
                .WithMany()
                .HasForeignKey(rr => rr.ReviewedByUserId)
                .WillCascadeOnDelete(false);
        }
    }
}

