using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ReviewReportRepository : Repository<ReviewReport>, IReviewReportRepository
    {
        public ReviewReportRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ReviewReport> GetByReviewId(int reviewId)
        {
            return _dbSet
                .Include(r => r.User)
                .Where(r => r.ProductReviewId == reviewId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewReport> GetByCommentId(int commentId)
        {
            return _dbSet
                .Include(r => r.User)
                .Where(r => r.ReviewCommentId == commentId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewReport> GetPendingReports()
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ProductReview)
                .Include(r => r.ReviewComment)
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewReport> GetByReportType(string reportType)
        {
            return _dbSet
                .Include(r => r.User)
                .Include(r => r.ProductReview)
                .Include(r => r.ReviewComment)
                .Where(r => r.ReportType == reportType)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public int GetReportCount(int reviewId)
        {
            return _dbSet.Count(r => r.ProductReviewId == reviewId);
        }

        public int GetReportCountForComment(int commentId)
        {
            return _dbSet.Count(r => r.ReviewCommentId == commentId);
        }

        public bool HasUserReported(int userId, int? reviewId, int? commentId)
        {
            if (reviewId.HasValue)
            {
                return _dbSet.Any(r => r.UserId == userId && r.ProductReviewId == reviewId);
            }
            if (commentId.HasValue)
            {
                return _dbSet.Any(r => r.UserId == userId && r.ReviewCommentId == commentId);
            }
            return false;
        }
    }
}

