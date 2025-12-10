using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class CouponUsageRepository : Repository<CouponUsage>, ICouponUsageRepository
    {
        public CouponUsageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<CouponUsage> GetByCouponId(int couponId)
        {
            return _dbSet
                .Include(cu => cu.Coupon)
                .Include(cu => cu.User)
                .Include(cu => cu.Order)
                .Where(cu => cu.CouponId == couponId)
                .OrderByDescending(cu => cu.UsedDate)
                .ToList();
        }

        public IEnumerable<CouponUsage> GetByUserId(int userId)
        {
            return _dbSet
                .Include(cu => cu.Coupon)
                .Include(cu => cu.Order)
                .Where(cu => cu.UserId == userId)
                .OrderByDescending(cu => cu.UsedDate)
                .ToList();
        }

        public IEnumerable<CouponUsage> GetByOrderId(int orderId)
        {
            return _dbSet
                .Include(cu => cu.Coupon)
                .Include(cu => cu.User)
                .Where(cu => cu.OrderId == orderId)
                .ToList();
        }

        public bool HasUserUsedCoupon(int userId, int couponId)
        {
            return _dbSet.Any(cu => cu.UserId == userId && cu.CouponId == couponId);
        }

        public int GetUsageCount(int couponId)
        {
            return _dbSet.Count(cu => cu.CouponId == couponId);
        }

        public int GetUsageCountForUser(int userId, int couponId)
        {
            return _dbSet.Count(cu => cu.UserId == userId && cu.CouponId == couponId);
        }
    }
}

