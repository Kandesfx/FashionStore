using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Coupon GetByCode(string code)
        {
            return _dbSet
                .SingleOrDefault(c => c.Code == code);
        }

        public IEnumerable<Coupon> GetActiveCoupons()
        {
            var now = DateTime.Now;
            return _dbSet
                .Where(c => c.IsActive &&
                           c.StartDate <= now &&
                           c.EndDate >= now)
                .OrderByDescending(c => c.StartDate)
                .ToList();
        }

        public IEnumerable<Coupon> GetCouponsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbSet
                .Where(c => (c.StartDate >= startDate && c.StartDate <= endDate) ||
                           (c.EndDate >= startDate && c.EndDate <= endDate) ||
                           (c.StartDate <= startDate && c.EndDate >= endDate))
                .OrderByDescending(c => c.StartDate)
                .ToList();
        }

        public IEnumerable<Coupon> GetCouponsByStatus(string status)
        {
            // Coupon không có trường Status; dùng IsActive như trạng thái
            bool isActive = status == "Active";
            return _dbSet
                .Where(c => c.IsActive == isActive)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public bool CodeExists(string code, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return _dbSet.Any(c => c.Code == code && c.Id != excludeId.Value);
            }
            return _dbSet.Any(c => c.Code == code);
        }

        public int GetUsageCount(int couponId)
        {
            return _context.Set<CouponUsage>()
                .Count(cu => cu.CouponId == couponId);
        }

        public int GetUsageCountForUser(int couponId, int userId)
        {
            return _context.Set<CouponUsage>()
                .Count(cu => cu.CouponId == couponId && cu.UserId == userId);
        }
    }
}

