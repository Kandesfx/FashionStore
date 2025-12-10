using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ReviewImageRepository : Repository<ReviewImage>, IReviewImageRepository
    {
        public ReviewImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ReviewImage> GetByReviewId(int reviewId)
        {
            return _dbSet
                .Where(img => img.ProductReviewId == reviewId)
                .OrderBy(img => img.DisplayOrder)
                .ToList();
        }

        public IEnumerable<ReviewImage> GetByProductId(int productId)
        {
            return _dbSet
                .Include(img => img.ProductReview)
                .Where(img => img.ProductReview.ProductId == productId)
                .OrderByDescending(img => img.ProductReview.CreatedDate)
                .ThenBy(img => img.DisplayOrder)
                .ToList();
        }
    }
}

