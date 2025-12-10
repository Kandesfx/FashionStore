using FashionStore.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Repositories.Interfaces
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        // Get reviews by product
        IEnumerable<ProductReview> GetByProductId(int productId, bool includeApprovedOnly = true);
        
        // Get reviews by user
        IEnumerable<ProductReview> GetByUserId(int userId);
        
        // Get reviews by status
        IEnumerable<ProductReview> GetByStatus(string status);
        
        // Get reviews by order (for verified purchase)
        IEnumerable<ProductReview> GetByOrderId(int orderId);
        
        // Get pending reviews (for moderation)
        IEnumerable<ProductReview> GetPendingReviews();
        
        // Get reported reviews
        IEnumerable<ProductReview> GetReportedReviews();
        
        // Get reviews with images
        IEnumerable<ProductReview> GetReviewsWithImages(int productId);
        
        // Get average rating for product
        double GetAverageRating(int productId);
        
        // Get rating distribution (1-5 stars)
        Dictionary<int, int> GetRatingDistribution(int productId);
        
        // Check if user has reviewed product
        bool HasUserReviewedProduct(int userId, int productId);
        
        // Get review by user and product
        ProductReview GetByUserAndProduct(int userId, int productId);
        
        // Get top helpful reviews
        IEnumerable<ProductReview> GetTopHelpfulReviews(int productId, int count = 10);
        
        // Get verified purchase reviews
        IEnumerable<ProductReview> GetVerifiedPurchaseReviews(int productId);
    }
}

