using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionStore.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository,
            IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public Order GetById(int id)
        {
            return _orderRepository.GetById(id);
        }

        public IEnumerable<Order> GetAll()
        {
            return _orderRepository.GetAll();
        }

        public IEnumerable<Order> GetOrdersByUser(int userId)
        {
            return _orderRepository.Find(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate);
        }

        public void CreateOrder(Order order, List<OrderDetail> orderDetails)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm");

            // Validate order
            if (!ValidateOrder(orderDetails))
                throw new InvalidOperationException("Đơn hàng không hợp lệ");

            // Calculate total
            order.TotalAmount = CalculateOrderTotal(orderDetails);
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            // Create order
            _orderRepository.Add(order);
            _unitOfWork.Complete();

            // Create order details and update stock
            foreach (var detail in orderDetails)
            {
                detail.OrderId = order.Id;
                _orderDetailRepository.Add(detail);
                
                // Update product stock
                var product = _productRepository.GetById(detail.ProductId);
                if (product.Stock < detail.Quantity)
                    throw new InvalidOperationException($"Sản phẩm {product.ProductName} không đủ tồn kho");
                
                product.Stock -= detail.Quantity;
                _productRepository.Update(product);
            }

            _unitOfWork.Complete();
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            var order = GetById(orderId);
            if (order == null)
                throw new InvalidOperationException("Đơn hàng không tồn tại");

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
                throw new ArgumentException("Trạng thái không hợp lệ");

            order.Status = status;
            _orderRepository.Update(order);
            _unitOfWork.Complete();
        }

        public decimal CalculateOrderTotal(List<OrderDetail> orderDetails)
        {
            return orderDetails.Sum(od => od.SubTotal);
        }

        public bool ValidateOrder(List<OrderDetail> orderDetails)
        {
            foreach (var detail in orderDetails)
            {
                var product = _productRepository.GetById(detail.ProductId);
                if (product == null || !product.IsActive)
                    return false;
                if (product.Stock < detail.Quantity)
                    return false;
            }
            return true;
        }

        public IEnumerable<Order> GetOrdersByStatus(string status)
        {
            return _orderRepository.Find(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate);
        }

        public OrderStatistics GetOrderStatistics()
        {
            var orders = GetAll().ToList();
            return new OrderStatistics
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalAmount),
                PendingOrders = orders.Count(o => o.Status == "Pending"),
                CompletedOrders = orders.Count(o => o.Status == "Delivered")
            };
        }
    }
}

