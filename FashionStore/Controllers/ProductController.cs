using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Services.Interfaces;
using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;

namespace FashionStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductReviewService _reviewService;

        public ProductController(IProductService productService, ICategoryService categoryService, IProductReviewService reviewService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
        }

        // GET: Product
        public ActionResult Index(int? categoryId, string sortBy, int page = 1)
        {
            var products = _productService.GetActiveProducts().ToList();
            
            // Filter by category
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            
            // Sorting
            switch (sortBy)
            {
                case "price-asc":
                    products = products.OrderBy(p => p.FinalPrice).ToList();
                    break;
                case "price-desc":
                    products = products.OrderByDescending(p => p.FinalPrice).ToList();
                    break;
                case "name-asc":
                    products = products.OrderBy(p => p.ProductName).ToList();
                    break;
                case "name-desc":
                    products = products.OrderByDescending(p => p.ProductName).ToList();
                    break;
                default:
                    products = products.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
            }
            
            // Pagination
            int pageSize = 12;
            int totalCount = products.Count;
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            ViewBag.Categories = _categoryService.GetActiveCategories();
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Products = pagedProducts;
            
            return View();
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            var product = _productService.GetById(id);
            if (product == null || !product.IsActive)
            {
                return HttpNotFound();
            }

            // Get related products (same category)
            var relatedProducts = _productService.GetProductsByCategory(product.CategoryId)
                .Where(p => p.Id != id)
                .Take(4)
                .ToList();

            // Get reviews
            var reviews = _reviewService.GetApprovedByProductId(id);
            var averageRating = _reviewService.GetAverageRating(id);
            var reviewCount = _reviewService.GetReviewCount(id);
            
            // Check if current user has reviewed this product
            var userId = GetCurrentUserId();
            ProductReview userReview = null;
            if (userId > 0)
            {
                userReview = _reviewService.GetByProductAndUser(id, userId);
            }

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.UserReview = userReview;
            ViewBag.CurrentUserId = userId;
            
            return View(product);
        }

        // POST: Product/SubmitReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SubmitReview(ProductReviewViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để đánh giá sản phẩm." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errors });
                }

                // Kiểm tra xem user đã đánh giá chưa
                var existingReview = _reviewService.GetByProductAndUser(model.ProductId, userId);
                if (existingReview != null)
                {
                    return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi." });
                }

                var review = new ProductReview
                {
                    ProductId = model.ProductId,
                    UserId = userId,
                    Rating = model.Rating,
                    Title = model.Title,
                    ReviewText = model.ReviewText,
                    Status = "Approved",
                    CreatedDate = DateTime.Now
                };

                _reviewService.Add(review);

                return Json(new { success = true, message = "Cảm ơn bạn đã đánh giá sản phẩm!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Product/SubmitComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SubmitComment(ReviewCommentViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để bình luận." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errors });
                }

                var comment = new ReviewComment
                {
                    ProductReviewId = model.ProductReviewId,
                    UserId = userId,
                    CommentText = model.CommentText,
                    Status = "Approved",
                    CreatedDate = DateTime.Now
                };

                _reviewService.AddComment(comment);

                return Json(new { success = true, message = "Bình luận đã được gửi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            return Session["UserId"] != null ? (int)Session["UserId"] : 0;
        }

        // GET: Product/Search
        public ActionResult Search(string q, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            var products = _productService.SearchProducts(q).ToList();
            
            int pageSize = 12;
            int totalCount = products.Count;
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            ViewBag.SearchTerm = q;
            ViewBag.Products = pagedProducts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            
            return View();
        }
    }
}

