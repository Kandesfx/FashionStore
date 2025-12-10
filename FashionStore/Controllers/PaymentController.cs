using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.Momo;
using FashionStore.Models.ViewModels;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    [AuthorizeRole("User", "Admin")]
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMomoPaymentService _momoPaymentService;
        private readonly ICartService _cartService;

        public PaymentController(IOrderService orderService, IMomoPaymentService momoPaymentService, ICartService cartService)
        {
            _orderService = orderService;
            _momoPaymentService = momoPaymentService;
            _cartService = cartService;
        }

        // Trang hiển thị QR/PayUrl MoMo
        public ActionResult MomoQr()
        {
            var model = TempData["MomoPayment"] as MomoPaymentViewModel;
            if (model == null || string.IsNullOrEmpty(model.PayUrl))
            {
                TempData["Error"] = "Không tìm thấy thông tin thanh toán MoMo. Vui lòng thử lại.";
                return RedirectToAction("Checkout", "Order");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult MomoIpn(MomoIpnRequest request)
        {
            // Log để debug
            System.Diagnostics.Debug.WriteLine("=== MOMO IPN CALLED ===");
            System.Diagnostics.Debug.WriteLine($"OrderId: {request?.OrderId}, ResultCode: {request?.ResultCode}");
            
            if (!_momoPaymentService.ValidateIpn(request))
            {
                System.Diagnostics.Debug.WriteLine("IPN validation failed - Invalid signature");
                return Json(new
                {
                    partnerCode = request?.PartnerCode,
                    requestId = request?.RequestId,
                    orderId = request?.OrderId,
                    errorCode = 5,
                    message = "Invalid signature"
                });
            }
            
            System.Diagnostics.Debug.WriteLine("IPN validation passed");

            // Tìm đơn hàng
            if (!int.TryParse(request.OrderId, out var orderId))
            {
                return Json(new
                {
                    partnerCode = request.PartnerCode,
                    requestId = request.RequestId,
                    orderId = request.OrderId,
                    errorCode = 6,
                    message = "Invalid orderId"
                });
            }

            var order = _orderService.GetById(orderId);
            if (order == null)
            {
                return Json(new
                {
                    partnerCode = request.PartnerCode,
                    requestId = request.RequestId,
                    orderId = request.OrderId,
                    errorCode = 7,
                    message = "Order not found"
                });
            }

            // Đối chiếu số tiền (MoMo trả về Amount là long VND, Order.TotalAmount là decimal)
            var orderAmountInVnd = (long)order.TotalAmount;
            if (orderAmountInVnd != request.Amount)
            {
                _orderService.UpdateOrderStatus(orderId, "Failed");
                return Json(new
                {
                    partnerCode = request.PartnerCode,
                    requestId = request.RequestId,
                    orderId = request.OrderId,
                    errorCode = 8,
                    message = "Amount mismatch"
                });
            }

            if (request.ResultCode == 0)
            {
                System.Diagnostics.Debug.WriteLine($"Updating order {orderId} to Paid status");
                _orderService.UpdateOrderStatus(orderId, "Paid");
                // Clear cart when payment is successful
                if (order.UserId > 0)
                {
                    _cartService.ClearCart(order.UserId);
                }
                System.Diagnostics.Debug.WriteLine($"Order {orderId} updated to Paid successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Updating order {orderId} to Failed status (ResultCode: {request.ResultCode})");
                _orderService.UpdateOrderStatus(orderId, "Failed");
            }

            return Json(new
            {
                partnerCode = request.PartnerCode,
                requestId = request.RequestId,
                orderId = request.OrderId,
                errorCode = 0,
                message = "Successful"
            });
        }

        [AllowAnonymous]
        public ActionResult MomoReturn()
        {
            // Log để debug
            System.Diagnostics.Debug.WriteLine("=== MOMO RETURN CALLED ===");
            System.Diagnostics.Debug.WriteLine($"QueryString: {Request.QueryString}");
            
            // Xử lý kết quả từ redirect URL và cập nhật trạng thái đơn hàng
            var resultCode = Request.QueryString["resultCode"];
            var message = Request.QueryString["message"];
            var orderIdStr = Request.QueryString["orderId"];
            
            System.Diagnostics.Debug.WriteLine($"ResultCode: {resultCode}, OrderId: {orderIdStr}, Message: {message}");

            ViewBag.ResultCode = resultCode;
            ViewBag.Message = message;
            ViewBag.OrderId = orderIdStr;

            // Lấy thông tin đơn hàng nếu có orderId
            if (!string.IsNullOrEmpty(orderIdStr) && int.TryParse(orderIdStr, out var orderId))
            {
                var order = _orderService.GetById(orderId);
                if (order != null)
                {
                    // Cập nhật trạng thái đơn hàng dựa trên resultCode từ redirect URL
                    // (IPN có thể không được gọi nếu dùng localhost)
                    if (string.Equals(resultCode, "0"))
                    {
                        // Thanh toán thành công
                        System.Diagnostics.Debug.WriteLine($"MomoReturn: Updating order {orderId} to Paid status");
                        if (order.Status != "Paid")
                        {
                            _orderService.UpdateOrderStatus(orderId, "Paid");
                            // Clear cart when payment is successful
                            if (order.UserId > 0)
                            {
                                _cartService.ClearCart(order.UserId);
                            }
                            System.Diagnostics.Debug.WriteLine($"MomoReturn: Order {orderId} updated to Paid successfully");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"MomoReturn: Order {orderId} already Paid");
                        }
                        ViewBag.Message = "Thanh toán đã được xử lý thành công. Đơn hàng của bạn đang được chuẩn bị.";
                    }
                    else if (!string.IsNullOrEmpty(resultCode) && !string.Equals(resultCode, "0"))
                    {
                        // Thanh toán thất bại
                        if (order.Status == "Pending")
                        {
                            _orderService.UpdateOrderStatus(orderId, "Failed");
                        }
                        ViewBag.Message = "Thanh toán không thành công. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
                    }
                    else if (string.IsNullOrEmpty(message))
                    {
                        if (string.Equals(resultCode, "0"))
                        {
                            ViewBag.Message = "Thanh toán đã được xử lý thành công. Đơn hàng của bạn đang được chuẩn bị.";
                        }
                        else
                        {
                            ViewBag.Message = "Thanh toán không thành công. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
                        }
                    }

                    ViewBag.Order = order;
                }
            }

            return View();
        }
    }
}

