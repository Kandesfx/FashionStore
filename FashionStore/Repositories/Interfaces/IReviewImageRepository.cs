using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IReviewImageRepository : IRepository<ReviewImage>
    {
        // Get images by review
        IEnumerable<ReviewImage> GetByReviewId(int reviewId);
        
        // Get images by product (all reviews)
        IEnumerable<ReviewImage> GetByProductId(int productId);
    }
}

