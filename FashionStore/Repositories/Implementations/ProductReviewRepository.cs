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

        public IEnumerable<ProductReview> GetByProductId(int productId, bool includeApprovedOnly = true)
        {
            var query = _dbSet
                .Include(r => r.User)
                .Include(r => r.ReviewImages)
                .Include(r => r.ReviewHelpfuls)
                .Where(r => r.ProductId == productId);

            if (includeApprovedOnly)
            {
                query = query.Where(r => r.Status == "Approved");
            }

            return query
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ProductReview> GetByUserId(int userId)
        {
            return _dbSet
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ProductReview> GetByStatus(string status)
        {
            return _dbSet
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ProductReview> GetByOrderId(int orderId)
        {
            return _dbSet
                .Include(r => r.Product)
                .Where(r => r.OrderId == orderId)
                .ToList();
        }

        public IEnumerable<ProductReview> GetPendingReviews()
        {
            return GetByStatus("Pending");
        }

        public IEnumerable<ProductReview> GetReportedReviews()
        {
            return _dbSet
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => r.Status == "Reported" || r.ReportCount > 0)
                .OrderByDescending(r => r.ReportCount)
                .ThenByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ProductReview> GetReviewsWithImages(int productId)
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ReviewImages)
                .Where(r => r.ProductId == productId && r.Status == "Approved" && r.ReviewImages.Any())
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public double GetAverageRating(int productId)
        {
            var reviews = _dbSet
                .Where(r => r.ProductId == productId && r.Status == "Approved")
                .ToList();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => (double)r.Rating);
        }

        public Dictionary<int, int> GetRatingDistribution(int productId)
        {
            var reviews = _dbSet
                .Where(r => r.ProductId == productId && r.Status == "Approved")
                .ToList();

            var distribution = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 }
            };

            foreach (var review in reviews)
            {
                if (distribution.ContainsKey(review.Rating))
                {
                    distribution[review.Rating]++;
                }
            }

            return distribution;
        }

        public bool HasUserReviewedProduct(int userId, int productId)
        {
            return _dbSet.Any(r => r.UserId == userId && r.ProductId == productId);
        }

        public ProductReview GetByUserAndProduct(int userId, int productId)
        {
            return _dbSet
                .Include(r => r.Product)
                .Include(r => r.ReviewImages)
                .SingleOrDefault(r => r.UserId == userId && r.ProductId == productId);
        }

        public IEnumerable<ProductReview> GetTopHelpfulReviews(int productId, int count = 10)
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ReviewImages)
                .Where(r => r.ProductId == productId && r.Status == "Approved")
                .OrderByDescending(r => r.HelpfulCount)
                .ThenByDescending(r => r.CreatedDate)
                .Take(count)
                .ToList();
        }

        public IEnumerable<ProductReview> GetVerifiedPurchaseReviews(int productId)
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ReviewImages)
                .Where(r => r.ProductId == productId && r.Status == "Approved" && r.IsVerifiedPurchase)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }
    }
}

