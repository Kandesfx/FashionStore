using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IPromotionRepository : IRepository<Promotion>
    {
        // Get active promotions
        IEnumerable<Promotion> GetActivePromotions();
        
        // Get promotions by date range
        IEnumerable<Promotion> GetPromotionsByDateRange(DateTime startDate, DateTime endDate);
        
        // Get promotions by status
        IEnumerable<Promotion> GetPromotionsByStatus(string status);
        
        // Get promotions applicable to product
        IEnumerable<Promotion> GetPromotionsForProduct(int productId);
        
        // Get promotions applicable to category
        IEnumerable<Promotion> GetPromotionsForCategory(int categoryId);
    }
}

