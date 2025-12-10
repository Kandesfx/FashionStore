using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            // Calculate total (should already include ShippingFee from OrderController)
            // If ShippingFee is not set, calculate it here
            var subtotal = CalculateOrderTotal(orderDetails);
            if (order.ShippingFee == 0 && subtotal < 500000)
            {
                order.ShippingFee = 30000;
            }
            // Ensure TotalAmount includes ShippingFee
            if (order.TotalAmount != subtotal + order.ShippingFee)
            {
                order.TotalAmount = subtotal + order.ShippingFee;
            }
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

            // Hợp lệ cả các trạng thái thanh toán
            var validStatuses = new[] { "Pending", "Paid", "Failed", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
                throw new ArgumentException("Trạng thái không hợp lệ");

            order.Status = status;
            _orderRepository.Update(order);
            _unitOfWork.Complete();
        }

        public void UpdateShippingAddress(int orderId, string shippingAddress)
        {
            var order = GetById(orderId);
            if (order == null)
                throw new InvalidOperationException("Đơn hàng không tồn tại");

            if (order.Status != "Pending")
                throw new InvalidOperationException("Chỉ có thể sửa địa chỉ khi đơn hàng đang ở trạng thái Pending");

            if (string.IsNullOrWhiteSpace(shippingAddress))
                throw new ArgumentException("Địa chỉ giao hàng không được để trống");

            order.ShippingAddress = shippingAddress;
            _orderRepository.Update(order);
            _unitOfWork.Complete();
        }

        public void CancelOrder(int orderId)
        {
            var order = GetById(orderId);
            if (order == null)
                throw new InvalidOperationException("Đơn hàng không tồn tại");

            if (order.Status == "Cancelled")
                throw new InvalidOperationException("Đơn hàng đã được hủy trước đó");

            if (order.Status != "Pending" && order.Status != "Paid")
                throw new InvalidOperationException("Chỉ có thể hủy đơn hàng khi đang ở trạng thái Pending hoặc Paid");

            // Trả lại tồn kho cho các sản phẩm trong đơn hàng
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = _productRepository.GetById(detail.ProductId);
                    if (product != null)
                    {
                        product.Stock += detail.Quantity;
                        _productRepository.Update(product);
                    }
                }
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = "Cancelled";
            order.CancelledDate = DateTime.Now;
            _orderRepository.Update(order);
            _unitOfWork.Complete();
        }

        public decimal CalculateOrderTotal(List<OrderDetail> orderDetails)
        {
            // This method is kept for backward compatibility
            // TotalAmount should already include ShippingFee when passed from OrderController
            return orderDetails.Sum(od => od.SubTotal);
        }

        public bool ValidateOrder(List<OrderDetail> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
            {
                throw new InvalidOperationException("Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi đặt hàng.");
            }

            foreach (var detail in orderDetails)
            {
                if (detail == null)
                {
                    throw new InvalidOperationException("Thông tin sản phẩm không hợp lệ.");
                }

                if (detail.ProductId <= 0)
                {
                    throw new InvalidOperationException($"Mã sản phẩm không hợp lệ: {detail.ProductId}");
                }

                var product = _productRepository.GetById(detail.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm với mã: {detail.ProductId}");
                }
                
                if (!product.IsActive)
                {
                    throw new InvalidOperationException($"Sản phẩm '{product.ProductName}' đã ngừng bán.");
                }
                
                if (detail.Quantity <= 0)
                {
                    throw new InvalidOperationException($"Số lượng sản phẩm '{product.ProductName}' phải lớn hơn 0.");
                }
                
                if (product.Stock < detail.Quantity)
                {
                    throw new InvalidOperationException($"Sản phẩm '{product.ProductName}' chỉ còn {product.Stock} sản phẩm trong kho. Vui lòng giảm số lượng.");
                }
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

        public decimal GetRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            return _orderRepository.Find(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Delivered")
                .Sum(o => o.TotalAmount);
        }

        public int GetOrderCountByDateRange(DateTime startDate, DateTime endDate)
        {
            return _orderRepository.Find(o => o.OrderDate >= startDate && o.OrderDate <= endDate).Count();
        }

        public IEnumerable<TopProductViewModel> GetTopSellingProducts(int topCount = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var allOrderDetails = _orderDetailRepository.GetAll().ToList();
            var allOrders = _orderRepository.GetAll().ToList();
            var allProducts = _productRepository.GetAll().ToList();
            
            // Join order details with orders
            var orderDetailsWithOrders = allOrderDetails
                .Join(allOrders, 
                    od => od.OrderId, 
                    o => o.Id, 
                    (od, o) => new { OrderDetail = od, Order = o })
                .Where(x => x.Order.Status == "Delivered");
            
            if (startDate.HasValue && endDate.HasValue)
            {
                orderDetailsWithOrders = orderDetailsWithOrders
                    .Where(x => x.Order.OrderDate >= startDate.Value && x.Order.OrderDate <= endDate.Value);
            }

            // Join with products
            var orderDetailsWithProducts = orderDetailsWithOrders
                .Join(allProducts,
                    x => x.OrderDetail.ProductId,
                    p => p.Id,
                    (x, p) => new { x.OrderDetail, x.Order, Product = p });

            var topProducts = orderDetailsWithProducts
                .GroupBy(x => new { 
                    x.OrderDetail.ProductId, 
                    ProductName = x.Product.ProductName,
                    ImageUrl = x.Product.ImageUrl ?? ""
                })
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.OrderDetail.Quantity),
                    Revenue = g.Sum(x => x.OrderDetail.SubTotal),
                    ImageUrl = g.Key.ImageUrl
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(topCount)
                .ToList();

            return topProducts;
        }

        public List<RevenueChartData> GetRevenueByMonth(int months = 6)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-months);
            
            var orders = _orderRepository.Find(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Delivered")
                .ToList();

            var result = new List<RevenueChartData>();
            
            for (int i = months - 1; i >= 0; i--)
            {
                var monthStart = endDate.AddMonths(-i).Date;
                monthStart = new DateTime(monthStart.Year, monthStart.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var monthOrders = orders.Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd).ToList();
                
                result.Add(new RevenueChartData
                {
                    Month = monthStart.ToString("MM/yyyy"),
                    Revenue = monthOrders.Sum(o => o.TotalAmount),
                    OrderCount = monthOrders.Count
                });
            }

            return result;
        }

        public List<OrderChartData> GetOrdersByStatusChart()
        {
            var orders = GetAll().ToList();
            var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            
            return statuses.Select(status => new OrderChartData
            {
                Status = status,
                Count = orders.Count(o => o.Status == status)
            }).ToList();
        }
    }
}

