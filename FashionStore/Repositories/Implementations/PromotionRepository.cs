using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class PromotionRepository : Repository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Promotion> GetActivePromotions()
        {
            var now = DateTime.Now;
            return _dbSet
                .Where(p => p.IsActive &&
                           p.StartDate <= now &&
                           p.EndDate >= now)
                .OrderByDescending(p => p.StartDate)
                .ToList();
        }

        public IEnumerable<Promotion> GetPromotionsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbSet
                .Where(p => (p.StartDate >= startDate && p.StartDate <= endDate) ||
                           (p.EndDate >= startDate && p.EndDate <= endDate) ||
                           (p.StartDate <= startDate && p.EndDate >= endDate))
                .OrderByDescending(p => p.StartDate)
                .ToList();
        }

        public IEnumerable<Promotion> GetPromotionsByStatus(string status)
        {
            // Promotion không có trường Status; dùng IsActive như trạng thái
            bool isActive = status == "Active";
            return _dbSet
                .Where(p => p.IsActive == isActive)
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }

        public IEnumerable<Promotion> GetPromotionsForProduct(int productId)
        {
            var now = DateTime.Now;
            // For now, return all active promotions
            // Can be enhanced later with product-specific logic
            return _dbSet
                .Where(p => p.IsActive && 
                           p.StartDate <= now && 
                           p.EndDate >= now)
                .OrderByDescending(p => p.DiscountPercentage)
                .ThenByDescending(p => p.DiscountAmount)
                .ToList();
        }

        public IEnumerable<Promotion> GetPromotionsForCategory(int categoryId)
        {
            var now = DateTime.Now;
            // For now, return all active promotions
            // Can be enhanced later with category-specific logic
            return _dbSet
                .Where(p => p.IsActive && 
                           p.StartDate <= now && 
                           p.EndDate >= now)
                .OrderByDescending(p => p.DiscountPercentage)
                .ThenByDescending(p => p.DiscountAmount)
                .ToList();
        }
    }
}

