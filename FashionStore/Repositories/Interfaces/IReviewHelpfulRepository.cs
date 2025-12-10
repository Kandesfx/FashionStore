using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IReviewHelpfulRepository : IRepository<ReviewHelpful>
    {
        // Get helpful votes by review
        IEnumerable<ReviewHelpful> GetByReviewId(int reviewId);
        
        // Check if user has voted
        bool HasUserVoted(int userId, int reviewId);
        
        // Get vote by user and review
        ReviewHelpful GetByUserAndReview(int userId, int reviewId);
        
        // Get helpful count
        int GetHelpfulCount(int reviewId);
        
        // Get not helpful count
        int GetNotHelpfulCount(int reviewId);
    }
}

