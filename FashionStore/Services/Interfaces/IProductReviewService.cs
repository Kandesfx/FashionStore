using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IProductReviewService
    {
        // Get reviews
        IEnumerable<ProductReview> GetProductReviews(int productId, bool approvedOnly = true);
        ProductReview GetReviewById(int reviewId);
        IEnumerable<ProductReview> GetUserReviews(int userId);
        IEnumerable<ProductReview> GetPendingReviews();
        IEnumerable<ProductReview> GetReportedReviews();
        
        // Create/Update/Delete
        ProductReview CreateReview(CreateReviewViewModel model, int userId);
        void UpdateReview(int reviewId, UpdateReviewViewModel model, int userId);
        void DeleteReview(int reviewId, int userId);
        
        // Admin actions
        void ApproveReview(int reviewId, int adminUserId, string adminNotes = null);
        void RejectReview(int reviewId, int adminUserId, string reason);
        void MarkAsReported(int reviewId);
        
        // Statistics
        double GetAverageRating(int productId);
        Dictionary<int, int> GetRatingDistribution(int productId);
        int GetReviewCount(int productId, bool approvedOnly = true);
        
        // Helpful votes
        void VoteHelpful(int reviewId, int userId, bool isHelpful);
        bool HasUserVoted(int userId, int reviewId);
        
        // Comments
        ReviewComment AddComment(int reviewId, int userId, string commentText, int? parentCommentId = null);
        void DeleteComment(int commentId, int userId);
        IEnumerable<ReviewComment> GetReviewComments(int reviewId);
        
        // Reports
        void ReportReview(int reviewId, int userId, string reportType, string reason, string description);
        void ReportComment(int commentId, int userId, string reportType, string reason, string description);
        void ResolveReport(int reportId, int adminUserId, string adminNotes, bool isResolved);
        
        // Images
        void AddReviewImages(int reviewId, List<string> imageUrls);
        IEnumerable<ReviewImage> GetReviewImages(int reviewId);
        
        // Verification
        bool CanUserReviewProduct(int userId, int productId);
        bool VerifyPurchase(int userId, int productId, int? orderId = null);
    }
}

