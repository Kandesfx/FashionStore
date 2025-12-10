using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Repositories.Interfaces
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        IEnumerable<ProductReview> GetByProductId(int productId);
        IEnumerable<ProductReview> GetApprovedByProductId(int productId);
        ProductReview GetByProductAndUser(int productId, int userId);
        double GetAverageRating(int productId);
        int GetReviewCount(int productId);
    }
}

