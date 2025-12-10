using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Promotion GetById(int id)
        {
            return _unitOfWork.Promotions.GetById(id);
        }

        public IEnumerable<Promotion> GetAll()
        {
            return _unitOfWork.Promotions.GetAll();
        }

        public IEnumerable<Promotion> GetActivePromotions()
        {
            return _unitOfWork.Promotions.GetActivePromotions();
        }

        public void Create(Promotion promotion)
        {
            if (promotion == null)
                throw new ArgumentNullException(nameof(promotion));

            if (string.IsNullOrWhiteSpace(promotion.Name))
                throw new ArgumentException("Tên khuyến mãi không được để trống.");

            if (promotion.StartDate >= promotion.EndDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");

            if (promotion.DiscountPercentage.HasValue && (promotion.DiscountPercentage < 0 || promotion.DiscountPercentage > 100))
                throw new ArgumentException("Phần trăm giảm giá phải từ 0 đến 100.");

            if (promotion.DiscountAmount.HasValue && promotion.DiscountAmount < 0)
                throw new ArgumentException("Số tiền giảm giá phải >= 0.");

            promotion.CreatedDate = DateTime.Now;
            _unitOfWork.Promotions.Add(promotion);
            _unitOfWork.Complete();
        }

        public void Update(Promotion promotion)
        {
            if (promotion == null)
                throw new ArgumentNullException(nameof(promotion));

            var existingPromotion = _unitOfWork.Promotions.GetById(promotion.Id);
            if (existingPromotion == null)
                throw new ArgumentException("Khuyến mãi không tồn tại.");

            if (string.IsNullOrWhiteSpace(promotion.Name))
                throw new ArgumentException("Tên khuyến mãi không được để trống.");

            if (promotion.StartDate >= promotion.EndDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");

            existingPromotion.Name = promotion.Name;
            existingPromotion.Description = promotion.Description;
            existingPromotion.PromotionType = promotion.PromotionType;
            existingPromotion.DiscountPercentage = promotion.DiscountPercentage;
            existingPromotion.DiscountAmount = promotion.DiscountAmount;
            existingPromotion.MinimumOrderAmount = promotion.MinimumOrderAmount;
            existingPromotion.MaximumDiscountAmount = promotion.MaximumDiscountAmount;
            existingPromotion.StartDate = promotion.StartDate;
            existingPromotion.EndDate = promotion.EndDate;
            existingPromotion.UsageLimit = promotion.UsageLimit;
            existingPromotion.IsActive = promotion.IsActive;
            existingPromotion.UpdatedDate = DateTime.Now;

            _unitOfWork.Promotions.Update(existingPromotion);
            _unitOfWork.Complete();
        }

        public void Delete(int id)
        {
            var promotion = _unitOfWork.Promotions.GetById(id);
            if (promotion == null)
                throw new ArgumentException("Khuyến mãi không tồn tại.");

            _unitOfWork.Promotions.Remove(promotion);
            _unitOfWork.Complete();
        }

        public IEnumerable<Promotion> GetApplicablePromotions(int? productId = null, int? categoryId = null, decimal? orderAmount = null)
        {
            var promotions = GetActivePromotions().ToList();

            // Filter by product/category if specified
            if (productId.HasValue)
            {
                promotions = _unitOfWork.Promotions.GetPromotionsForProduct(productId.Value).ToList();
            }
            else if (categoryId.HasValue)
            {
                promotions = _unitOfWork.Promotions.GetPromotionsForCategory(categoryId.Value).ToList();
            }

            // Filter by minimum order amount
            if (orderAmount.HasValue)
            {
                promotions = promotions.Where(p => !p.MinimumOrderAmount.HasValue || p.MinimumOrderAmount <= orderAmount.Value).ToList();
            }

            // Filter by usage limit
            promotions = promotions.Where(p => !p.UsageLimit.HasValue || p.UsedCount < p.UsageLimit.Value).ToList();

            return promotions;
        }

        public decimal CalculateDiscount(int promotionId, decimal orderAmount)
        {
            var promotion = _unitOfWork.Promotions.GetById(promotionId);
            if (promotion == null)
                return 0;

            if (!IsPromotionApplicable(promotionId, orderAmount))
                return 0;

            decimal discount = 0;

            if (promotion.DiscountPercentage.HasValue)
            {
                discount = orderAmount * promotion.DiscountPercentage.Value / 100;
            }
            else if (promotion.DiscountAmount.HasValue)
            {
                discount = promotion.DiscountAmount.Value;
            }

            // Apply maximum discount limit
            if (promotion.MaximumDiscountAmount.HasValue && discount > promotion.MaximumDiscountAmount.Value)
            {
                discount = promotion.MaximumDiscountAmount.Value;
            }

            // Don't exceed order amount
            if (discount > orderAmount)
            {
                discount = orderAmount;
            }

            return discount;
        }

        public Promotion GetBestPromotion(decimal orderAmount, int? productId = null, int? categoryId = null)
        {
            var applicablePromotions = GetApplicablePromotions(productId, categoryId, orderAmount).ToList();

            if (!applicablePromotions.Any())
                return null;

            Promotion bestPromotion = null;
            decimal maxDiscount = 0;

            foreach (var promotion in applicablePromotions)
            {
                decimal discount = CalculateDiscount(promotion.Id, orderAmount);
                if (discount > maxDiscount)
                {
                    maxDiscount = discount;
                    bestPromotion = promotion;
                }
            }

            return bestPromotion;
        }

        public bool IsPromotionValid(int promotionId)
        {
            var promotion = _unitOfWork.Promotions.GetById(promotionId);
            if (promotion == null)
                return false;

            var now = DateTime.Now;
            if (!promotion.IsActive || promotion.StartDate > now || promotion.EndDate < now)
                return false;

            if (promotion.UsageLimit.HasValue && promotion.UsedCount >= promotion.UsageLimit.Value)
                return false;

            return true;
        }

        public bool IsPromotionApplicable(int promotionId, decimal orderAmount, int? productId = null, int? categoryId = null)
        {
            if (!IsPromotionValid(promotionId))
                return false;

            var promotion = _unitOfWork.Promotions.GetById(promotionId);
            if (promotion == null)
                return false;

            // Check minimum order amount
            if (promotion.MinimumOrderAmount.HasValue && orderAmount < promotion.MinimumOrderAmount.Value)
                return false;

            return true;
        }
    }
}

