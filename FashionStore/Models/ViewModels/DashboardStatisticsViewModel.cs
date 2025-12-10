using System;
using System.Collections.Generic;

namespace FashionStore.Models.ViewModels
{
    public class DashboardStatisticsViewModel
    {
        // Order Statistics
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        
        // Revenue by period
        public decimal TodayRevenue { get; set; }
        public decimal ThisWeekRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal LastMonthRevenue { get; set; }
        
        // Orders by period
        public int TodayOrders { get; set; }
        public int ThisWeekOrders { get; set; }
        public int ThisMonthOrders { get; set; }
        
        // Product Statistics
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        
        // User Statistics
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        
        // Review Statistics
        public int TotalReviews { get; set; }
        public int PendingReviews { get; set; }
        public double AverageRating { get; set; }
        
        // Promotion & Coupon Statistics
        public int ActivePromotions { get; set; }
        public int ActiveCoupons { get; set; }
        public int CouponUsagesThisMonth { get; set; }
        
        // Top Products
        public List<TopProductViewModel> TopSellingProducts { get; set; }
        
        // Revenue Chart Data
        public List<RevenueChartData> RevenueByMonth { get; set; }
        public List<OrderChartData> OrdersByStatus { get; set; }
    }
    
    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public string ImageUrl { get; set; }
    }
    
    public class RevenueChartData
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
    
    public class OrderChartData
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}

