using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IPromotionService
    {
        // CRUD
        Promotion GetById(int id);
        IEnumerable<Promotion> GetAll();
        IEnumerable<Promotion> GetActivePromotions();
        void Create(Promotion promotion);
        void Update(Promotion promotion);
        void Delete(int id);
        
        // Get applicable promotions
        IEnumerable<Promotion> GetApplicablePromotions(int? productId = null, int? categoryId = null, decimal? orderAmount = null);
        
        // Calculate discount
        decimal CalculateDiscount(int promotionId, decimal orderAmount);
        Promotion GetBestPromotion(decimal orderAmount, int? productId = null, int? categoryId = null);
        
        // Validation
        bool IsPromotionValid(int promotionId);
        bool IsPromotionApplicable(int promotionId, decimal orderAmount, int? productId = null, int? categoryId = null);
    }
}

