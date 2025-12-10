using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using FashionStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ProductReviewService(IProductReviewRepository reviewRepository, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public IEnumerable<ProductReview> GetByProductId(int productId)
        {
            return _reviewRepository.GetByProductId(productId);
        }

        public IEnumerable<ProductReview> GetApprovedByProductId(int productId)
        {
            return _reviewRepository.GetApprovedByProductId(productId);
        }

        public ProductReview GetById(int id)
        {
            return _reviewRepository.GetById(id);
        }

        public ProductReview GetByProductAndUser(int productId, int userId)
        {
            return _reviewRepository.GetByProductAndUser(productId, userId);
        }

        public void Add(ProductReview review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            if (review.Rating < 1 || review.Rating > 5)
                throw new ArgumentException("Điểm đánh giá phải từ 1 đến 5");

            // Kiểm tra xem user đã đánh giá sản phẩm này chưa
            var existingReview = GetByProductAndUser(review.ProductId, review.UserId);
            if (existingReview != null)
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi");

            review.Status = "Approved"; // Tự động approve, có thể thay đổi thành "Pending" nếu cần admin duyệt
            review.CreatedDate = DateTime.Now;

            _reviewRepository.Add(review);
            _unitOfWork.Complete();
        }

        public void Update(ProductReview review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            var existingReview = _reviewRepository.GetById(review.Id);
            if (existingReview == null)
                throw new InvalidOperationException("Đánh giá không tồn tại");

            existingReview.Rating = review.Rating;
            existingReview.Title = review.Title;
            existingReview.ReviewText = review.ReviewText;
            existingReview.UpdatedDate = DateTime.Now;

            _reviewRepository.Update(existingReview);
            _unitOfWork.Complete();
        }

        public void Delete(int id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
                throw new InvalidOperationException("Đánh giá không tồn tại");

            _reviewRepository.Remove(review);
            _unitOfWork.Complete();
        }

        public double GetAverageRating(int productId)
        {
            return _reviewRepository.GetAverageRating(productId);
        }

        public int GetReviewCount(int productId)
        {
            return _reviewRepository.GetReviewCount(productId);
        }

        public void AddComment(ReviewComment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            if (string.IsNullOrWhiteSpace(comment.CommentText))
                throw new ArgumentException("Nội dung bình luận không được để trống");

            // Kiểm tra review có tồn tại không
            var review = _reviewRepository.GetById(comment.ProductReviewId);
            if (review == null)
                throw new InvalidOperationException("Đánh giá không tồn tại");

            comment.Status = "Approved";
            comment.CreatedDate = DateTime.Now;

            // Thêm comment trực tiếp vào DbSet
            _context.ReviewComments.Add(comment);
            _unitOfWork.Complete();
        }
    }
}

