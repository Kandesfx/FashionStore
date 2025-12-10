using FashionStore.Data;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FashionStore.Repositories.Implementations
{
    public class ReviewHelpfulRepository : Repository<ReviewHelpful>, IReviewHelpfulRepository
    {
        public ReviewHelpfulRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ReviewHelpful> GetByReviewId(int reviewId)
        {
            return _dbSet
                .Include(v => v.User)
                .Where(v => v.ProductReviewId == reviewId)
                .ToList();
        }

        public bool HasUserVoted(int userId, int reviewId)
        {
            return _dbSet.Any(v => v.UserId == userId && v.ProductReviewId == reviewId);
        }

        public ReviewHelpful GetByUserAndReview(int userId, int reviewId)
        {
            return _dbSet
                .SingleOrDefault(v => v.UserId == userId && v.ProductReviewId == reviewId);
        }

        public int GetHelpfulCount(int reviewId)
        {
            return _dbSet.Count(v => v.ProductReviewId == reviewId && v.IsHelpful);
        }

        public int GetNotHelpfulCount(int reviewId)
        {
            return _dbSet.Count(v => v.ProductReviewId == reviewId && !v.IsHelpful);
        }
    }
}

