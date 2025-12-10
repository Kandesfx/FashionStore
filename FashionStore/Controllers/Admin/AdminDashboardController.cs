using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.ViewModels;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IProductReviewService _reviewService;
        private readonly IInventoryService _inventoryService;
        private readonly IPromotionService _promotionService;
        private readonly ICouponService _couponService;

        public AdminDashboardController(
            IOrderService orderService, 
            IProductService productService, 
            IUserService userService,
            IProductReviewService reviewService,
            IInventoryService inventoryService,
            IPromotionService promotionService,
            ICouponService couponService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
            _reviewService = reviewService;
            _inventoryService = inventoryService;
            _promotionService = promotionService;
            _couponService = couponService;
        }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            var now = DateTime.Now;
            var today = now.Date;
            var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var lastMonthEnd = thisMonthStart.AddDays(-1);

            var statistics = _orderService.GetOrderStatistics();
            var allOrders = _orderService.GetAll().ToList();
            var recentOrders = _orderService.GetOrdersByStatus("Pending").Take(10);

            var dashboardStats = new DashboardStatisticsViewModel
            {
                // Order Statistics
                TotalOrders = statistics.TotalOrders,
                TotalRevenue = statistics.TotalRevenue,
                PendingOrders = statistics.PendingOrders,
                ProcessingOrders = allOrders.Count(o => o.Status == "Processing"),
                ShippedOrders = allOrders.Count(o => o.Status == "Shipped"),
                CompletedOrders = statistics.CompletedOrders,
                CancelledOrders = allOrders.Count(o => o.Status == "Cancelled"),

                // Revenue by period
                TodayRevenue = _orderService.GetRevenueByDateRange(today, now),
                ThisWeekRevenue = _orderService.GetRevenueByDateRange(thisWeekStart, now),
                ThisMonthRevenue = _orderService.GetRevenueByDateRange(thisMonthStart, now),
                LastMonthRevenue = _orderService.GetRevenueByDateRange(lastMonthStart, lastMonthEnd),

                // Orders by period
                TodayOrders = _orderService.GetOrderCountByDateRange(today, now),
                ThisWeekOrders = _orderService.GetOrderCountByDateRange(thisWeekStart, now),
                ThisMonthOrders = _orderService.GetOrderCountByDateRange(thisMonthStart, now),

                // Product Statistics
                TotalProducts = _productService.GetAll().Count(),
                ActiveProducts = _productService.GetActiveProducts().Count(),
                LowStockProducts = _inventoryService.GetLowStockProducts().Count(),
                OutOfStockProducts = _productService.GetAll().Count(p => p.Stock <= 0),

                // User Statistics
                TotalUsers = _userService.GetAll().Count(),
                NewUsersThisMonth = _userService.GetAll().Count(u => u.CreatedDate >= thisMonthStart),

                // Review Statistics
                TotalReviews = _reviewService.GetProductReviews(0, false).Count(),
                PendingReviews = _reviewService.GetPendingReviews().Count(),
                AverageRating = CalculateAverageRating(),

                // Promotion & Coupon Statistics
                ActivePromotions = _promotionService.GetActivePromotions().Count(),
                ActiveCoupons = _couponService.GetActiveCoupons().Count(),
                CouponUsagesThisMonth = GetCouponUsagesThisMonth(thisMonthStart, now),

                // Top Products
                TopSellingProducts = _orderService.GetTopSellingProducts(5).ToList(),

                // Chart Data
                RevenueByMonth = _orderService.GetRevenueByMonth(6),
                OrdersByStatus = _orderService.GetOrdersByStatusChart()
            };

            ViewBag.DashboardStats = dashboardStats;
            ViewBag.RecentOrders = recentOrders;

            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }

        private double CalculateAverageRating()
        {
            var allReviews = _reviewService.GetProductReviews(0, true).ToList();
            if (!allReviews.Any())
                return 0;

            return allReviews.Average(r => (double)r.Rating);
        }

        private int GetCouponUsagesThisMonth(DateTime startDate, DateTime endDate)
        {
            // This is a simplified version - you may need to implement a method in CouponService
            // For now, return 0 or implement based on your CouponUsage entity
            return 0; // TODO: Implement if CouponUsage has date tracking
        }
    }
}

