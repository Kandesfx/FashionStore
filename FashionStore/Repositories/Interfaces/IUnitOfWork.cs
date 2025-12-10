using System;

namespace FashionStore.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IOrderRepository Orders { get; }
        IOrderDetailRepository OrderDetails { get; }
        IUserRepository Users { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IRoleRepository Roles { get; }
        IProductReviewRepository ProductReviews { get; }
        IReviewCommentRepository ReviewComments { get; }
        IReviewImageRepository ReviewImages { get; }
        IReviewHelpfulRepository ReviewHelpfuls { get; }
        IReviewReportRepository ReviewReports { get; }
        IInventoryTransactionRepository InventoryTransactions { get; }
        IPromotionRepository Promotions { get; }
        ICouponRepository Coupons { get; }
        ICouponUsageRepository CouponUsages { get; }
        IProductVariantRepository ProductVariants { get; }
        
        int Complete();
    }
}

