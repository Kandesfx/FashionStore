using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        // Get coupon by code
        Coupon GetByCode(string code);
        
        // Get active coupons
        IEnumerable<Coupon> GetActiveCoupons();
        
        // Get coupons by date range
        IEnumerable<Coupon> GetCouponsByDateRange(DateTime startDate, DateTime endDate);
        
        // Get coupons by status
        IEnumerable<Coupon> GetCouponsByStatus(string status);
        
        // Check if coupon code exists
        bool CodeExists(string code, int? excludeId = null);
        
        // Get coupon usage count
        int GetUsageCount(int couponId);
        
        // Get coupon usage count for user
        int GetUsageCountForUser(int couponId, int userId);
    }
}

