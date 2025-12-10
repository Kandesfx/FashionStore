using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Coupon GetById(int id)
        {
            return _unitOfWork.Coupons.GetById(id);
        }

        public Coupon GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return _unitOfWork.Coupons.GetByCode(code.Trim().ToUpper());
        }

        public IEnumerable<Coupon> GetAll()
        {
            return _unitOfWork.Coupons.GetAll();
        }

        public IEnumerable<Coupon> GetActiveCoupons()
        {
            return _unitOfWork.Coupons.GetActiveCoupons();
        }

        public void Create(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException(nameof(coupon));

            if (string.IsNullOrWhiteSpace(coupon.Code))
                throw new ArgumentException("Mã giảm giá không được để trống.");

            coupon.Code = coupon.Code.Trim().ToUpper();

            if (_unitOfWork.Coupons.CodeExists(coupon.Code))
                throw new ArgumentException("Mã giảm giá đã tồn tại.");

            if (coupon.StartDate >= coupon.EndDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");

            if (coupon.DiscountType == "Percentage")
            {
                if (!coupon.DiscountPercentage.HasValue || coupon.DiscountPercentage < 0 || coupon.DiscountPercentage > 100)
                    throw new ArgumentException("Phần trăm giảm giá phải từ 0 đến 100.");
            }
            else if (coupon.DiscountType == "FixedAmount")
            {
                if (!coupon.DiscountAmount.HasValue || coupon.DiscountAmount < 0)
                    throw new ArgumentException("Số tiền giảm giá phải >= 0.");
            }

            coupon.CreatedDate = DateTime.Now;
            _unitOfWork.Coupons.Add(coupon);
            _unitOfWork.Complete();
        }

        public void Update(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException(nameof(coupon));

            var existingCoupon = _unitOfWork.Coupons.GetById(coupon.Id);
            if (existingCoupon == null)
                throw new ArgumentException("Mã giảm giá không tồn tại.");

            if (string.IsNullOrWhiteSpace(coupon.Code))
                throw new ArgumentException("Mã giảm giá không được để trống.");

            coupon.Code = coupon.Code.Trim().ToUpper();

            if (_unitOfWork.Coupons.CodeExists(coupon.Code, coupon.Id))
                throw new ArgumentException("Mã giảm giá đã tồn tại.");

            if (coupon.StartDate >= coupon.EndDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");

            existingCoupon.Code = coupon.Code;
            existingCoupon.Name = coupon.Name;
            existingCoupon.Description = coupon.Description;
            existingCoupon.DiscountType = coupon.DiscountType;
            existingCoupon.DiscountPercentage = coupon.DiscountPercentage;
            existingCoupon.DiscountAmount = coupon.DiscountAmount;
            existingCoupon.MinimumOrderAmount = coupon.MinimumOrderAmount;
            existingCoupon.MaximumDiscountAmount = coupon.MaximumDiscountAmount;
            existingCoupon.StartDate = coupon.StartDate;
            existingCoupon.EndDate = coupon.EndDate;
            existingCoupon.UsageLimit = coupon.UsageLimit;
            existingCoupon.UsageLimitPerUser = coupon.UsageLimitPerUser;
            existingCoupon.IsActive = coupon.IsActive;
            existingCoupon.UpdatedDate = DateTime.Now;

            _unitOfWork.Coupons.Update(existingCoupon);
            _unitOfWork.Complete();
        }

        public void Delete(int id)
        {
            var coupon = _unitOfWork.Coupons.GetById(id);
            if (coupon == null)
                throw new ArgumentException("Mã giảm giá không tồn tại.");

            _unitOfWork.Coupons.Remove(coupon);
            _unitOfWork.Complete();
        }

        public bool IsCouponValid(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            var coupon = GetByCode(code);
            if (coupon == null)
                return false;

            var now = DateTime.Now;
            if (!coupon.IsActive || coupon.StartDate > now || coupon.EndDate < now)
                return false;

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                return false;

            return true;
        }

        public bool IsCouponApplicable(string code, decimal orderAmount, int userId)
        {
            if (!IsCouponValid(code))
                return false;

            var coupon = GetByCode(code);
            if (coupon == null)
                return false;

            // Check minimum order amount
            if (coupon.MinimumOrderAmount.HasValue && orderAmount < coupon.MinimumOrderAmount.Value)
                return false;

            // Check usage limit per user
            if (coupon.UsageLimitPerUser.HasValue)
            {
                int userUsageCount = _unitOfWork.CouponUsages.GetUsageCountForUser(userId, coupon.Id);
                if (userUsageCount >= coupon.UsageLimitPerUser.Value)
                    return false;
            }

            return true;
        }

        public bool CanUserUseCoupon(int userId, int couponId)
        {
            var coupon = _unitOfWork.Coupons.GetById(couponId);
            if (coupon == null)
                return false;

            if (!IsCouponValid(coupon.Code))
                return false;

            if (coupon.UsageLimitPerUser.HasValue)
            {
                int userUsageCount = _unitOfWork.CouponUsages.GetUsageCountForUser(userId, couponId);
                if (userUsageCount >= coupon.UsageLimitPerUser.Value)
                    return false;
            }

            return true;
        }

        public decimal CalculateDiscount(string code, decimal orderAmount)
        {
            var coupon = GetByCode(code);
            if (coupon == null)
                return 0;

            if (!IsCouponValid(code))
                return 0;

            decimal discount = 0;

            if (coupon.DiscountType == "Percentage" && coupon.DiscountPercentage.HasValue)
            {
                discount = orderAmount * coupon.DiscountPercentage.Value / 100;
            }
            else if (coupon.DiscountType == "FixedAmount" && coupon.DiscountAmount.HasValue)
            {
                discount = coupon.DiscountAmount.Value;
            }

            // Apply maximum discount limit
            if (coupon.MaximumDiscountAmount.HasValue && discount > coupon.MaximumDiscountAmount.Value)
            {
                discount = coupon.MaximumDiscountAmount.Value;
            }

            // Don't exceed order amount
            if (discount > orderAmount)
            {
                discount = orderAmount;
            }

            return discount;
        }

        public void RecordUsage(int couponId, int orderId, int userId, decimal discountAmount)
        {
            var coupon = _unitOfWork.Coupons.GetById(couponId);
            if (coupon == null)
                throw new ArgumentException("Mã giảm giá không tồn tại.");

            var usage = new CouponUsage
            {
                CouponId = couponId,
                OrderId = orderId,
                UserId = userId,
                DiscountAmount = discountAmount,
                UsedDate = DateTime.Now
            };

            _unitOfWork.CouponUsages.Add(usage);

            // Update coupon usage count
            coupon.UsedCount++;
            _unitOfWork.Coupons.Update(coupon);

            _unitOfWork.Complete();
        }

        public int GetUsageCount(int couponId)
        {
            return _unitOfWork.Coupons.GetUsageCount(couponId);
        }

        public int GetUsageCountForUser(int userId, int couponId)
        {
            return _unitOfWork.CouponUsages.GetUsageCountForUser(userId, couponId);
        }

        public IEnumerable<CouponUsage> GetCouponUsages(int couponId)
        {
            return _unitOfWork.CouponUsages.GetByCouponId(couponId);
        }
    }
}

