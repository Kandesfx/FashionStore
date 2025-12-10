using FashionStore.Data;
using FashionStore.Repositories.Implementations;
using FashionStore.Repositories.Interfaces;
using System;

namespace FashionStore.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        private IProductRepository _products;
        private ICategoryRepository _categories;
        private IOrderRepository _orders;
        private IOrderDetailRepository _orderDetails;
        private IUserRepository _users;
        private ICartRepository _carts;
        private ICartItemRepository _cartItems;
        private IRoleRepository _roles;
        private IProductReviewRepository _productReviews;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products
        {
            get
            {
                return _products ?? (_products = new ProductRepository(_context));
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                return _categories ?? (_categories = new CategoryRepository(_context));
            }
        }

        public IOrderRepository Orders
        {
            get
            {
                return _orders ?? (_orders = new OrderRepository(_context));
            }
        }

        public IOrderDetailRepository OrderDetails
        {
            get
            {
                return _orderDetails ?? (_orderDetails = new OrderDetailRepository(_context));
            }
        }

        public IUserRepository Users
        {
            get
            {
                return _users ?? (_users = new UserRepository(_context));
            }
        }

        public ICartRepository Carts
        {
            get
            {
                return _carts ?? (_carts = new CartRepository(_context));
            }
        }

        public ICartItemRepository CartItems
        {
            get
            {
                return _cartItems ?? (_cartItems = new CartItemRepository(_context));
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                return _roles ?? (_roles = new RoleRepository(_context));
            }
        }

        public IProductReviewRepository ProductReviews
        {
            get
            {
                return _productReviews ?? (_productReviews = new ProductReviewRepository(_context));
            }
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

