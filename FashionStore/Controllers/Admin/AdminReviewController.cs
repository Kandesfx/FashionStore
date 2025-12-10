using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.ViewModels;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminReviewController : Controller
    {
        private readonly IProductReviewService _reviewService;
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminReviewController(IProductReviewService reviewService, IProductService productService, IUnitOfWork unitOfWork)
        {
            _reviewService = reviewService;
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Review
        public ActionResult Index(string status = "All", int page = 1)
        {
            int pageSize = 20;
            var reviews = status == "All" 
                ? _reviewService.GetProductReviews(0, false).ToList() // Get all reviews
                : status == "Pending"
                    ? _reviewService.GetPendingReviews().ToList()
                    : status == "Reported"
                        ? _reviewService.GetReportedReviews().ToList()
                        : _reviewService.GetProductReviews(0, false)
                            .Where(r => r.Status == status).ToList();

            // Pagination
            int totalCount = reviews.Count;
            var pagedReviews = reviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Reviews = pagedReviews;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Status = status;
            ViewBag.Statuses = new[] { "All", "Pending", "Approved", "Rejected", "Reported" };

            return View("~/Views/Admin/Review/Index.cshtml");
        }

        // GET: Admin/Review/Details/5
        public ActionResult Details(int id)
        {
            var review = _reviewService.GetReviewById(id);
            if (review == null)
            {
                return HttpNotFound();
            }

            ViewBag.Review = review;
            ViewBag.Comments = _reviewService.GetReviewComments(id);
            ViewBag.Reports = GetReportsForReview(id);

            return View("~/Views/Admin/Review/Details.cshtml");
        }

        // POST: Admin/Review/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id, string adminNotes = null)
        {
            try
            {
                int adminUserId = GetCurrentUserId();
                _reviewService.ApproveReview(id, adminUserId, adminNotes);
                TempData["SuccessMessage"] = "Đánh giá đã được duyệt thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi duyệt đánh giá: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/Review/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id, string reason)
        {
            try
            {
                int adminUserId = GetCurrentUserId();
                _reviewService.RejectReview(id, adminUserId, reason);
                TempData["SuccessMessage"] = "Đánh giá đã bị từ chối.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi từ chối đánh giá: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/Review/BulkApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkApprove(int[] reviewIds)
        {
            if (reviewIds == null || reviewIds.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một đánh giá.";
                return RedirectToAction("Index");
            }

            int adminUserId = GetCurrentUserId();
            int successCount = 0;

            foreach (var reviewId in reviewIds)
            {
                try
                {
                    _reviewService.ApproveReview(reviewId, adminUserId);
                    successCount++;
                }
                catch
                {
                    // Continue with other reviews
                }
            }

            TempData["SuccessMessage"] = $"Đã duyệt {successCount}/{reviewIds.Length} đánh giá.";
            return RedirectToAction("Index");
        }

        // POST: Admin/Review/BulkReject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkReject(int[] reviewIds, string reason)
        {
            if (reviewIds == null || reviewIds.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một đánh giá.";
                return RedirectToAction("Index");
            }

            int adminUserId = GetCurrentUserId();
            int successCount = 0;

            foreach (var reviewId in reviewIds)
            {
                try
                {
                    _reviewService.RejectReview(reviewId, adminUserId, reason);
                    successCount++;
                }
                catch
                {
                    // Continue with other reviews
                }
            }

            TempData["SuccessMessage"] = $"Đã từ chối {successCount}/{reviewIds.Length} đánh giá.";
            return RedirectToAction("Index");
        }

        // POST: Admin/Review/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                int adminUserId = GetCurrentUserId();
                _reviewService.DeleteReview(id, adminUserId);
                TempData["SuccessMessage"] = "Đánh giá đã được xóa.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa đánh giá: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Review/Statistics
        public ActionResult Statistics(int? productId = null)
        {
            if (productId.HasValue)
            {
                var product = _productService.GetById(productId.Value);
                if (product == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Product = product;
                ViewBag.AverageRating = _reviewService.GetAverageRating(productId.Value);
                ViewBag.RatingDistribution = _reviewService.GetRatingDistribution(productId.Value);
                ViewBag.TotalReviews = _reviewService.GetReviewCount(productId.Value);
            }
            else
            {
                // Overall statistics
                var allReviews = _reviewService.GetProductReviews(0, false).ToList();
                ViewBag.TotalReviews = allReviews.Count();
                ViewBag.PendingReviews = allReviews.Count(r => r.Status == "Pending");
                ViewBag.ReportedReviews = allReviews.Count(r => r.Status == "Reported");
                
                // Calculate overall average rating
                if (allReviews.Any())
                {
                    ViewBag.AverageRating = allReviews.Where(r => r.Status == "Approved").Average(r => (double?)r.Rating) ?? 0.0;
                }
                else
                {
                    ViewBag.AverageRating = 0.0;
                }
            }

            return View("~/Views/Admin/Review/Statistics.cshtml");
        }

        // POST: Admin/Review/ReplyToReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyToReview(int reviewId, string replyText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(replyText))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập nội dung phản hồi.";
                    return RedirectToAction("Details", new { id = reviewId });
                }

                int adminUserId = GetCurrentUserId();
                _reviewService.AddComment(reviewId, adminUserId, replyText);
                TempData["SuccessMessage"] = "Phản hồi đã được thêm thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi thêm phản hồi: " + ex.Message;
            }

            return RedirectToAction("Details", new { id = reviewId });
        }

        // POST: Admin/Review/ApproveComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveComment(int commentId, int reviewId)
        {
            try
            {
                var comment = _unitOfWork.ReviewComments.GetById(commentId);
                if (comment != null)
                {
                    comment.Status = "Approved";
                    _unitOfWork.ReviewComments.Update(comment);
                    _unitOfWork.Complete();
                    TempData["SuccessMessage"] = "Bình luận đã được duyệt.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi duyệt bình luận: " + ex.Message;
            }

            return RedirectToAction("Details", new { id = reviewId });
        }

        // POST: Admin/Review/RejectComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectComment(int commentId, int reviewId, string reason)
        {
            try
            {
                var comment = _unitOfWork.ReviewComments.GetById(commentId);
                if (comment != null)
                {
                    comment.Status = "Rejected";
                    _unitOfWork.ReviewComments.Update(comment);
                    _unitOfWork.Complete();
                    TempData["SuccessMessage"] = "Bình luận đã bị từ chối.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi từ chối bình luận: " + ex.Message;
            }

            return RedirectToAction("Details", new { id = reviewId });
        }

        // POST: Admin/Review/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int commentId, int reviewId)
        {
            try
            {
                int adminUserId = GetCurrentUserId();
                _reviewService.DeleteComment(commentId, adminUserId);
                TempData["SuccessMessage"] = "Bình luận đã được xóa.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa bình luận: " + ex.Message;
            }

            return RedirectToAction("Details", new { id = reviewId });
        }

        // GET: Admin/Review/Reports
        public ActionResult Reports(string status = "Pending", int page = 1)
        {
            int pageSize = 20;
            var reports = GetReports(status).ToList();

            int totalCount = reports.Count;
            var pagedReports = reports.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Reports = pagedReports;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Status = status;
            ViewBag.Statuses = new[] { "Pending", "Reviewed", "Resolved", "Dismissed" };

            return View("~/Views/Admin/Review/Reports.cshtml");
        }

        // POST: Admin/Review/ResolveReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResolveReport(int reportId, string adminNotes, bool isResolved)
        {
            try
            {
                int adminUserId = GetCurrentUserId();
                _reviewService.ResolveReport(reportId, adminUserId, adminNotes, isResolved);
                TempData["SuccessMessage"] = isResolved ? "Báo cáo đã được xử lý." : "Báo cáo đã bị bỏ qua.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xử lý báo cáo: " + ex.Message;
            }

            return RedirectToAction("Reports");
        }

        private int GetCurrentUserId()
        {
            // Get current user ID from session or claims
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            throw new UnauthorizedAccessException("User not authenticated");
        }

        private System.Collections.Generic.IEnumerable<FashionStore.Models.Entities.ReviewReport> GetReports(string status)
        {
            if (status == "Pending")
            {
                return _unitOfWork.ReviewReports.GetPendingReports();
            }
            return _unitOfWork.ReviewReports.GetAll().Where(r => r.Status == status);
        }

        private System.Collections.Generic.IEnumerable<FashionStore.Models.Entities.ReviewReport> GetReportsForReview(int reviewId)
        {
            return _unitOfWork.ReviewReports.GetByReviewId(reviewId);
        }
    }
}

