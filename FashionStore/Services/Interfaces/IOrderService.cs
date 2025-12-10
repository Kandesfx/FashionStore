using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using System.Collections.Generic;

namespace FashionStore.Services.Interfaces
{
    public interface IOrderService
    {
        Order GetById(int id);
        IEnumerable<Order> GetAll();
        IEnumerable<Order> GetOrdersByUser(int userId);
        void CreateOrder(Order order, List<OrderDetail> orderDetails);
        void UpdateOrderStatus(int orderId, string status);
        void UpdateShippingAddress(int orderId, string shippingAddress);
        void CancelOrder(int orderId);
        
        decimal CalculateOrderTotal(List<OrderDetail> orderDetails);
        bool ValidateOrder(List<OrderDetail> orderDetails);
        IEnumerable<Order> GetOrdersByStatus(string status);
        OrderStatistics GetOrderStatistics();
    }

    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
    }
}

