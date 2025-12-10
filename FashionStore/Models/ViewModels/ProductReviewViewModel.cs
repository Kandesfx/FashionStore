using FashionStore.Models.Entities;
using System;
using System.Collections.Generic;

namespace FashionStore.Models.ViewModels
{
    public class ProductReviewViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string ReviewText { get; set; }
        public string Status { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ReviewImageViewModel> Images { get; set; }
        public List<ReviewCommentViewModel> Comments { get; set; }
        public bool HasUserVoted { get; set; }
        public bool? UserVoteIsHelpful { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ReviewImageViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ReviewCommentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string CommentText { get; set; }
        public bool IsAdminReply { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ReviewCommentViewModel> Replies { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ReviewStatisticsViewModel
    {
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; }
        public int VerifiedPurchaseCount { get; set; }
        public int ReviewsWithImagesCount { get; set; }
    }
}

