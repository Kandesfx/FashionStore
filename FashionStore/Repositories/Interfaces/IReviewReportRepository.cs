using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IReviewReportRepository : IRepository<ReviewReport>
    {
        // Get reports by review
        IEnumerable<ReviewReport> GetByReviewId(int reviewId);
        
        // Get reports by comment
        IEnumerable<ReviewReport> GetByCommentId(int commentId);
        
        // Get pending reports
        IEnumerable<ReviewReport> GetPendingReports();
        
        // Get reports by type
        IEnumerable<ReviewReport> GetByReportType(string reportType);
        
        // Get report count for review
        int GetReportCount(int reviewId);
        
        // Get report count for comment
        int GetReportCountForComment(int commentId);
        
        // Check if user has reported
        bool HasUserReported(int userId, int? reviewId, int? commentId);
    }
}

