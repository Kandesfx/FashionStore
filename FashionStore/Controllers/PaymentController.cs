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

        public PaymentController(IOrderService orderService, IMomoPaymentService momoPaymentService)
        {
            _orderService = orderService;
            _momoPaymentService = momoPaymentService;
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
            if (!_momoPaymentService.ValidateIpn(request))
            {
                return Json(new
                {
                    partnerCode = request?.PartnerCode,
                    requestId = request?.RequestId,
                    orderId = request?.OrderId,
                    errorCode = 5,
                    message = "Invalid signature"
                });
            }

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

            // Đối chiếu số tiền
            if (order.TotalAmount != request.Amount)
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
                _orderService.UpdateOrderStatus(orderId, "Paid");
            }
            else
            {
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
            // Chỉ hiển thị, trạng thái đã xử lý qua IPN
            ViewBag.ResultCode = Request.QueryString["resultCode"];
            ViewBag.Message = Request.QueryString["message"];
            ViewBag.OrderId = Request.QueryString["orderId"];
            return View();
        }
    }
}

