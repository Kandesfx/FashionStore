using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ReviewCommentRepository : Repository<ReviewComment>, IReviewCommentRepository
    {
        public ReviewCommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ReviewComment> GetByReviewId(int reviewId, bool includeReplies = true)
        {
            var query = _dbSet
                .Include(c => c.User)
                .Where(c => c.ProductReviewId == reviewId);

            if (includeReplies)
            {
                query = query.Include(c => c.ChildComments);
            }

            return query
                .Where(c => c.ParentCommentId == null) // Only top-level comments
                .OrderBy(c => c.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewComment> GetByUserId(int userId)
        {
            return _dbSet
                .Include(c => c.ProductReview)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewComment> GetPendingComments()
        {
            return _dbSet
                .Include(c => c.ProductReview)
                .Include(c => c.User)
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewComment> GetReportedComments()
        {
            return _dbSet
                .Include(c => c.ProductReview)
                .Include(c => c.User)
                .Where(c => c.ReportCount > 0)
                .OrderByDescending(c => c.ReportCount)
                .ThenByDescending(c => c.CreatedDate)
                .ToList();
        }

        public IEnumerable<ReviewComment> GetAdminReplies(int reviewId)
        {
            return _dbSet
                .Include(c => c.User)
                .Where(c => c.ProductReviewId == reviewId && c.IsAdminReply)
                .OrderBy(c => c.CreatedDate)
                .ToList();
        }

        public ReviewComment GetCommentWithReplies(int commentId)
        {
            return _dbSet
                .Include(c => c.User)
                .Include(c => c.ChildComments.Select(cc => cc.User))
                .SingleOrDefault(c => c.Id == commentId);
        }
    }
}

