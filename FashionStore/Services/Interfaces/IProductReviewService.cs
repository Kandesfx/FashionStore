using FashionStore.Models.Entities;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IProductReviewService
    {
        IEnumerable<ProductReview> GetByProductId(int productId);
        IEnumerable<ProductReview> GetApprovedByProductId(int productId);
        ProductReview GetById(int id);
        ProductReview GetByProductAndUser(int productId, int userId);
        void Add(ProductReview review);
        void Update(ProductReview review);
        void Delete(int id);
        double GetAverageRating(int productId);
        int GetReviewCount(int productId);
        void AddComment(ReviewComment comment);
    }
}

