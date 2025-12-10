using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface ICouponService
    {
        // CRUD
        Coupon GetById(int id);
        Coupon GetByCode(string code);
        IEnumerable<Coupon> GetAll();
        IEnumerable<Coupon> GetActiveCoupons();
        void Create(Coupon coupon);
        void Update(Coupon coupon);
        void Delete(int id);
        
        // Validation
        bool IsCouponValid(string code);
        bool IsCouponApplicable(string code, decimal orderAmount, int userId);
        bool CanUserUseCoupon(int userId, int couponId);
        
        // Calculate discount
        decimal CalculateDiscount(string code, decimal orderAmount);
        
        // Usage
        void RecordUsage(int couponId, int orderId, int userId, decimal discountAmount);
        
        // Statistics
        int GetUsageCount(int couponId);
        int GetUsageCountForUser(int userId, int couponId);
        IEnumerable<CouponUsage> GetCouponUsages(int couponId);
    }
}

