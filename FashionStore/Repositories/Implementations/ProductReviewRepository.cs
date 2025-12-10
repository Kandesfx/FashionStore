using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ProductReview> GetByProductId(int productId)
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ReviewComments)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ProductReview> GetApprovedByProductId(int productId)
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ReviewComments.Select(c => c.User))
                .Where(r => r.ProductId == productId && r.Status == "Approved")
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public ProductReview GetByProductAndUser(int productId, int userId)
        {
            return _dbSet
                .Include(r => r.User)
                .FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);
        }

        public double GetAverageRating(int productId)
        {
            var reviews = _dbSet
                .Where(r => r.ProductId == productId && r.Status == "Approved")
                .ToList();

            if (reviews.Count == 0)
                return 0;

            return reviews.Average(r => (double)r.Rating);
        }

        public int GetReviewCount(int productId)
        {
            return _dbSet
                .Count(r => r.ProductId == productId && r.Status == "Approved");
        }
    }
}

