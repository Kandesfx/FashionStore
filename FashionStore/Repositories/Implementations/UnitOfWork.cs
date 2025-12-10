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
        private IReviewCommentRepository _reviewComments;
        private IReviewImageRepository _reviewImages;
        private IReviewHelpfulRepository _reviewHelpfuls;
        private IReviewReportRepository _reviewReports;
        private IInventoryTransactionRepository _inventoryTransactions;
        private IPromotionRepository _promotions;
        private ICouponRepository _coupons;
        private ICouponUsageRepository _couponUsages;
        private IProductVariantRepository _productVariants;

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

        public IReviewCommentRepository ReviewComments
        {
            get
            {
                return _reviewComments ?? (_reviewComments = new ReviewCommentRepository(_context));
            }
        }

        public IReviewImageRepository ReviewImages
        {
            get
            {
                return _reviewImages ?? (_reviewImages = new ReviewImageRepository(_context));
            }
        }

        public IReviewHelpfulRepository ReviewHelpfuls
        {
            get
            {
                return _reviewHelpfuls ?? (_reviewHelpfuls = new ReviewHelpfulRepository(_context));
            }
        }

        public IReviewReportRepository ReviewReports
        {
            get
            {
                return _reviewReports ?? (_reviewReports = new ReviewReportRepository(_context));
            }
        }

        public IInventoryTransactionRepository InventoryTransactions
        {
            get
            {
                return _inventoryTransactions ?? (_inventoryTransactions = new InventoryTransactionRepository(_context));
            }
        }

        public IPromotionRepository Promotions
        {
            get
            {
                return _promotions ?? (_promotions = new PromotionRepository(_context));
            }
        }

        public ICouponRepository Coupons
        {
            get
            {
                return _coupons ?? (_coupons = new CouponRepository(_context));
            }
        }

        public ICouponUsageRepository CouponUsages
        {
            get
            {
                return _couponUsages ?? (_couponUsages = new CouponUsageRepository(_context));
            }
        }

        public IProductVariantRepository ProductVariants
        {
            get
            {
                return _productVariants ?? (_productVariants = new ProductVariantRepository(_context));
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

