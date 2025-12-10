using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Models.ViewModels;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IProductReviewService _reviewService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public ReviewController(
            IProductReviewService reviewService, 
            IProductService productService,
            IOrderService orderService)
        {
            _reviewService = reviewService;
            _productService = productService;
            _orderService = orderService;
        }

        // GET: Review/Product/{productId}
        [AllowAnonymous]
        public ActionResult Product(int productId, string sortBy = "Newest", int page = 1)
        {
            var product = _productService.GetById(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var reviews = _reviewService.GetProductReviews(productId, approvedOnly: true).ToList();

            // Sorting
            switch (sortBy)
            {
                case "Helpful":
                    reviews = reviews.OrderByDescending(r => r.HelpfulCount).ToList();
                    break;
                case "Highest":
                    reviews = reviews.OrderByDescending(r => r.Rating).ToList();
                    break;
                case "Lowest":
                    reviews = reviews.OrderBy(r => r.Rating).ToList();
                    break;
                case "Verified":
                    reviews = reviews.OrderByDescending(r => r.IsVerifiedPurchase).ThenByDescending(r => r.CreatedDate).ToList();
                    break;
                default: // Newest
                    reviews = reviews.OrderByDescending(r => r.CreatedDate).ToList();
                    break;
            }

            // Pagination
            int pageSize = 10;
            int totalCount = reviews.Count;
            var pagedReviews = reviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Statistics
            var statistics = new ReviewStatisticsViewModel
            {
                ProductId = productId,
                AverageRating = _reviewService.GetAverageRating(productId),
                TotalReviews = totalCount,
                RatingDistribution = _reviewService.GetRatingDistribution(productId),
                VerifiedPurchaseCount = reviews.Count(r => r.IsVerifiedPurchase),
                ReviewsWithImagesCount = reviews.Count(r => r.ReviewImages != null && r.ReviewImages.Any())
            };

            ViewBag.Product = product;
            ViewBag.Reviews = pagedReviews;
            ViewBag.Statistics = statistics;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.SortBy = sortBy;

            return View("~/Views/Review/Product.cshtml");
        }

        // GET: Review/Create/{productId}
        public ActionResult Create(int productId, int? orderId = null)
        {
            var product = _productService.GetById(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            int userId = GetCurrentUserId();

            // Check if user can review
            if (!_reviewService.CanUserReviewProduct(userId, productId))
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // Verify purchase if orderId provided
            bool isVerifiedPurchase = false;
            if (orderId.HasValue)
            {
                isVerifiedPurchase = _reviewService.VerifyPurchase(userId, productId, orderId);
            }

            ViewBag.Product = product;
            ViewBag.OrderId = orderId;
            ViewBag.IsVerifiedPurchase = isVerifiedPurchase;

            var model = new CreateReviewViewModel
            {
                ProductId = productId,
                OrderId = orderId,
                IsVerifiedPurchase = isVerifiedPurchase
            };

            return View("~/Views/Review/Create.cshtml", model);
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var product = _productService.GetById(model.ProductId);
                ViewBag.Product = product;
                return View("~/Views/Review/Create.cshtml", model);
            }

            try
            {
                int userId = GetCurrentUserId();
                var review = _reviewService.CreateReview(model, userId);
                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi và đang chờ duyệt.";
                return RedirectToAction("Product", new { productId = model.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo đánh giá: " + ex.Message;
                var product = _productService.GetById(model.ProductId);
                ViewBag.Product = product;
                return View("~/Views/Review/Create.cshtml", model);
            }
        }

        // GET: Review/Edit/{id}
        public ActionResult Edit(int id)
        {
            int userId = GetCurrentUserId();
            var review = _reviewService.GetReviewById(id);

            if (review == null)
            {
                return HttpNotFound();
            }

            if (review.UserId != userId)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa đánh giá này.";
                return RedirectToAction("Product", new { productId = review.ProductId });
            }

            var model = new UpdateReviewViewModel
            {
                Rating = review.Rating,
                Title = review.Title,
                ReviewText = review.ReviewText
            };

            ViewBag.Review = review;
            return View("~/Views/Review/Edit.cshtml", model);
        }

        // POST: Review/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UpdateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var review = _reviewService.GetReviewById(id);
                ViewBag.Review = review;
                return View("~/Views/Review/Edit.cshtml", model);
            }

            try
            {
                int userId = GetCurrentUserId();
                _reviewService.UpdateReview(id, model, userId);
                var review = _reviewService.GetReviewById(id);
                TempData["SuccessMessage"] = "Đánh giá đã được cập nhật và đang chờ duyệt lại.";
                return RedirectToAction("Product", new { productId = review.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật đánh giá: " + ex.Message;
                var review = _reviewService.GetReviewById(id);
                ViewBag.Review = review;
                return View("~/Views/Review/Edit.cshtml", model);
            }
        }

        // POST: Review/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                int userId = GetCurrentUserId();
                var review = _reviewService.GetReviewById(id);
                int productId = review.ProductId;
                _reviewService.DeleteReview(id, userId);
                TempData["SuccessMessage"] = "Đánh giá đã được xóa.";
                return RedirectToAction("Product", new { productId = productId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa đánh giá: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Review/VoteHelpful/{id}
        [HttpPost]
        public JsonResult VoteHelpful(int id, bool isHelpful)
        {
            try
            {
                int userId = GetCurrentUserId();
                _reviewService.VoteHelpful(id, userId, isHelpful);
                var review = _reviewService.GetReviewById(id);
                return Json(new { 
                    success = true, 
                    helpfulCount = review.HelpfulCount, 
                    notHelpfulCount = review.NotHelpfulCount 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Review/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int reviewId, string commentText, int? parentCommentId = null)
        {
            try
            {
                int userId = GetCurrentUserId();
                var comment = _reviewService.AddComment(reviewId, userId, commentText, parentCommentId);
                TempData["SuccessMessage"] = "Bình luận của bạn đã được gửi.";
                var review = _reviewService.GetReviewById(reviewId);
                return RedirectToAction("Product", new { productId = review.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi thêm bình luận: " + ex.Message;
                var review = _reviewService.GetReviewById(reviewId);
                return RedirectToAction("Product", new { productId = review.ProductId });
            }
        }

        // POST: Review/Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(int reviewId, string reportType, string reason, string description)
        {
            try
            {
                int userId = GetCurrentUserId();
                _reviewService.ReportReview(reviewId, userId, reportType, reason, description);
                TempData["SuccessMessage"] = "Cảm ơn bạn đã báo cáo. Chúng tôi sẽ xem xét.";
                var review = _reviewService.GetReviewById(reviewId);
                return RedirectToAction("Product", new { productId = review.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi báo cáo: " + ex.Message;
                var review = _reviewService.GetReviewById(reviewId);
                return RedirectToAction("Product", new { productId = review.ProductId });
            }
        }

        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            throw new UnauthorizedAccessException("User not authenticated");
        }
    }
}

