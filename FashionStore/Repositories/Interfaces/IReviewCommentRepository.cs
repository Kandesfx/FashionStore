using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IReviewCommentRepository : IRepository<ReviewComment>
    {
        // Get comments by review
        IEnumerable<ReviewComment> GetByReviewId(int reviewId, bool includeReplies = true);
        
        // Get comments by user
        IEnumerable<ReviewComment> GetByUserId(int userId);
        
        // Get pending comments (for moderation)
        IEnumerable<ReviewComment> GetPendingComments();
        
        // Get reported comments
        IEnumerable<ReviewComment> GetReportedComments();
        
        // Get admin replies
        IEnumerable<ReviewComment> GetAdminReplies(int reviewId);
        
        // Get comment with replies (thread)
        ReviewComment GetCommentWithReplies(int commentId);
    }
}

