using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface ICouponUsageRepository : IRepository<CouponUsage>
    {
        // Get usages by coupon
        IEnumerable<CouponUsage> GetByCouponId(int couponId);
        
        // Get usages by user
        IEnumerable<CouponUsage> GetByUserId(int userId);
        
        // Get usages by order
        IEnumerable<CouponUsage> GetByOrderId(int orderId);
        
        // Check if user has used coupon
        bool HasUserUsedCoupon(int userId, int couponId);
        
        // Get usage count for coupon
        int GetUsageCount(int couponId);
        
        // Get usage count for user
        int GetUsageCountForUser(int userId, int couponId);
    }
}

