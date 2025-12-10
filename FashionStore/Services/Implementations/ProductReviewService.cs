using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ProductReview> GetProductReviews(int productId, bool approvedOnly = true)
        {
            if (productId > 0)
            {
                return _unitOfWork.ProductReviews.GetByProductId(productId, approvedOnly);
            }
            else
            {
                // Get all reviews
                return approvedOnly 
                    ? _unitOfWork.ProductReviews.GetByStatus("Approved")
                    : _unitOfWork.ProductReviews.GetAll();
            }
        }

        public ProductReview GetReviewById(int reviewId)
        {
            return _unitOfWork.ProductReviews.GetById(reviewId);
        }

        public IEnumerable<ProductReview> GetUserReviews(int userId)
        {
            return _unitOfWork.ProductReviews.GetByUserId(userId);
        }

        public IEnumerable<ProductReview> GetPendingReviews()
        {
            return _unitOfWork.ProductReviews.GetPendingReviews();
        }

        public IEnumerable<ProductReview> GetReportedReviews()
        {
            return _unitOfWork.ProductReviews.GetReportedReviews();
        }

        public ProductReview CreateReview(CreateReviewViewModel model, int userId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check if user already reviewed this product
            if (_unitOfWork.ProductReviews.HasUserReviewedProduct(userId, model.ProductId))
            {
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi.");
            }

            // Verify purchase if OrderId is provided
            bool isVerifiedPurchase = false;
            if (model.OrderId.HasValue)
            {
                var order = _unitOfWork.Orders.GetById(model.OrderId.Value);
                if (order != null && order.UserId == userId && order.Status == "Completed")
                {
                    var orderDetail = _unitOfWork.OrderDetails.Find(od => 
                        od.OrderId == order.Id && od.ProductId == model.ProductId).FirstOrDefault();
                    if (orderDetail != null)
                    {
                        isVerifiedPurchase = true;
                    }
                }
            }

            var review = new ProductReview
            {
                ProductId = model.ProductId,
                UserId = userId,
                OrderId = model.OrderId,
                Rating = model.Rating,
                Title = model.Title,
                ReviewText = model.ReviewText,
                Status = "Pending", // Requires admin approval
                IsVerifiedPurchase = isVerifiedPurchase,
                CreatedDate = DateTime.Now
            };

            _unitOfWork.ProductReviews.Add(review);
            _unitOfWork.Complete();

            // Add images if provided
            if (model.ImageUrls != null && model.ImageUrls.Any())
            {
                AddReviewImages(review.Id, model.ImageUrls);
            }

            return review;
        }

        public void UpdateReview(int reviewId, UpdateReviewViewModel model, int userId)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                throw new ArgumentException("Đánh giá không tồn tại.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa đánh giá này.");

            if (review.Status != "Pending" && review.Status != "Approved")
                throw new InvalidOperationException("Không thể chỉnh sửa đánh giá đã bị từ chối.");

            review.Rating = model.Rating;
            review.Title = model.Title;
            review.ReviewText = model.ReviewText;
            review.UpdatedDate = DateTime.Now;
            review.Status = "Pending"; // Reset to pending after update

            _unitOfWork.ProductReviews.Update(review);
            _unitOfWork.Complete();
        }

        public void DeleteReview(int reviewId, int userId)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                throw new ArgumentException("Đánh giá không tồn tại.");

            // User can only delete their own reviews, or admin can delete any
            var user = _unitOfWork.Users.GetById(userId);
            bool isAdmin = user != null && user.Role != null && user.Role.RoleName == "Admin";

            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa đánh giá này.");

            _unitOfWork.ProductReviews.Remove(review);
            _unitOfWork.Complete();
        }

        public void ApproveReview(int reviewId, int adminUserId, string adminNotes = null)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                throw new ArgumentException("Đánh giá không tồn tại.");

            review.Status = "Approved";
            review.ReviewedByUserId = adminUserId;
            review.ReviewedDate = DateTime.Now;
            review.AdminNotes = adminNotes;
            review.UpdatedDate = DateTime.Now;

            _unitOfWork.ProductReviews.Update(review);
            _unitOfWork.Complete();
        }

        public void RejectReview(int reviewId, int adminUserId, string reason)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                throw new ArgumentException("Đánh giá không tồn tại.");

            review.Status = "Rejected";
            review.ReviewedByUserId = adminUserId;
            review.ReviewedDate = DateTime.Now;
            review.AdminNotes = reason;
            review.UpdatedDate = DateTime.Now;

            _unitOfWork.ProductReviews.Update(review);
            _unitOfWork.Complete();
        }

        public void MarkAsReported(int reviewId)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                return;

            review.Status = "Reported";
            review.ReportCount++;
            review.UpdatedDate = DateTime.Now;

            _unitOfWork.ProductReviews.Update(review);
            _unitOfWork.Complete();
        }

        public double GetAverageRating(int productId)
        {
            return _unitOfWork.ProductReviews.GetAverageRating(productId);
        }

        public Dictionary<int, int> GetRatingDistribution(int productId)
        {
            return _unitOfWork.ProductReviews.GetRatingDistribution(productId);
        }

        public int GetReviewCount(int productId, bool approvedOnly = true)
        {
            var reviews = _unitOfWork.ProductReviews.GetByProductId(productId, approvedOnly);
            return reviews.Count();
        }

        public void VoteHelpful(int reviewId, int userId, bool isHelpful)
        {
            // Check if user already voted
            var existingVote = _unitOfWork.ReviewHelpfuls.GetByUserAndReview(userId, reviewId);
            
            if (existingVote != null)
            {
                // Update existing vote
                existingVote.IsHelpful = isHelpful;
                _unitOfWork.ReviewHelpfuls.Update(existingVote);
            }
            else
            {
                // Create new vote
                var vote = new ReviewHelpful
                {
                    ProductReviewId = reviewId,
                    UserId = userId,
                    IsHelpful = isHelpful,
                    CreatedDate = DateTime.Now
                };
                _unitOfWork.ReviewHelpfuls.Add(vote);
            }

            // Update review counts
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review != null)
            {
                review.HelpfulCount = _unitOfWork.ReviewHelpfuls.GetHelpfulCount(reviewId);
                review.NotHelpfulCount = _unitOfWork.ReviewHelpfuls.GetNotHelpfulCount(reviewId);
                _unitOfWork.ProductReviews.Update(review);
            }

            _unitOfWork.Complete();
        }

        public bool HasUserVoted(int userId, int reviewId)
        {
            return _unitOfWork.ReviewHelpfuls.HasUserVoted(userId, reviewId);
        }

        public ReviewComment AddComment(int reviewId, int userId, string commentText, int? parentCommentId = null)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                throw new ArgumentException("Đánh giá không tồn tại.");

            var user = _unitOfWork.Users.GetById(userId);
            bool isAdmin = user != null && user.Role != null && user.Role.RoleName == "Admin";

            var comment = new ReviewComment
            {
                ProductReviewId = reviewId,
                UserId = userId,
                CommentText = commentText,
                ParentCommentId = parentCommentId,
                Status = isAdmin ? "Approved" : "Pending",
                IsAdminReply = isAdmin,
                CreatedDate = DateTime.Now
            };

            _unitOfWork.ReviewComments.Add(comment);
            _unitOfWork.Complete();

            return comment;
        }

        public void DeleteComment(int commentId, int userId)
        {
            var comment = _unitOfWork.ReviewComments.GetById(commentId);
            if (comment == null)
                throw new ArgumentException("Bình luận không tồn tại.");

            var user = _unitOfWork.Users.GetById(userId);
            bool isAdmin = user != null && user.Role != null && user.Role.RoleName == "Admin";

            if (comment.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này.");

            _unitOfWork.ReviewComments.Remove(comment);
            _unitOfWork.Complete();
        }

        public IEnumerable<ReviewComment> GetReviewComments(int reviewId)
        {
            return _unitOfWork.ReviewComments.GetByReviewId(reviewId);
        }

        public void ReportReview(int reviewId, int userId, string reportType, string reason, string description)
        {
            if (_unitOfWork.ReviewReports.HasUserReported(userId, reviewId, null))
            {
                throw new InvalidOperationException("Bạn đã báo cáo đánh giá này rồi.");
            }

            var report = new ReviewReport
            {
                ProductReviewId = reviewId,
                UserId = userId,
                ReportType = reportType,
                Reason = reason,
                Description = description,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            _unitOfWork.ReviewReports.Add(report);
            
            // Update review report count
            MarkAsReported(reviewId);
            
            _unitOfWork.Complete();
        }

        public void ReportComment(int commentId, int userId, string reportType, string reason, string description)
        {
            if (_unitOfWork.ReviewReports.HasUserReported(userId, null, commentId))
            {
                throw new InvalidOperationException("Bạn đã báo cáo bình luận này rồi.");
            }

            var comment = _unitOfWork.ReviewComments.GetById(commentId);
            if (comment == null)
                throw new ArgumentException("Bình luận không tồn tại.");

            var report = new ReviewReport
            {
                ReviewCommentId = commentId,
                UserId = userId,
                ReportType = reportType,
                Reason = reason,
                Description = description,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            _unitOfWork.ReviewReports.Add(report);
            
            // Update comment report count
            comment.ReportCount++;
            _unitOfWork.ReviewComments.Update(comment);
            
            _unitOfWork.Complete();
        }

        public void ResolveReport(int reportId, int adminUserId, string adminNotes, bool isResolved)
        {
            var report = _unitOfWork.ReviewReports.GetById(reportId);
            if (report == null)
                throw new ArgumentException("Báo cáo không tồn tại.");

            report.Status = isResolved ? "Resolved" : "Dismissed";
            report.ReviewedByUserId = adminUserId;
            report.ReviewedDate = DateTime.Now;
            report.AdminNotes = adminNotes;

            _unitOfWork.ReviewReports.Update(report);
            _unitOfWork.Complete();
        }

        public void AddReviewImages(int reviewId, List<string> imageUrls)
        {
            var review = _unitOfWork.ProductReviews.GetById(reviewId);
            if (review == null)
                throw new ArgumentException("Đánh giá không tồn tại.");

            int displayOrder = 0;
            foreach (var imageUrl in imageUrls)
            {
                var image = new ReviewImage
                {
                    ProductReviewId = reviewId,
                    ImageUrl = imageUrl,
                    DisplayOrder = displayOrder++,
                    CreatedDate = DateTime.Now
                };
                _unitOfWork.ReviewImages.Add(image);
            }

            _unitOfWork.Complete();
        }

        public IEnumerable<ReviewImage> GetReviewImages(int reviewId)
        {
            return _unitOfWork.ReviewImages.GetByReviewId(reviewId);
        }

        public bool CanUserReviewProduct(int userId, int productId)
        {
            // User can review if they haven't reviewed before
            return !_unitOfWork.ProductReviews.HasUserReviewedProduct(userId, productId);
        }

        public bool VerifyPurchase(int userId, int productId, int? orderId = null)
        {
            if (!orderId.HasValue)
                return false;

            var order = _unitOfWork.Orders.GetById(orderId.Value);
            if (order == null || order.UserId != userId || order.Status != "Completed")
                return false;

            var orderDetail = _unitOfWork.OrderDetails.Find(od => 
                od.OrderId == order.Id && od.ProductId == productId).FirstOrDefault();

            return orderDetail != null;
        }
    }
}

