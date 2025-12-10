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
        
        int Complete();
    }
}

