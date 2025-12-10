using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using System.Threading.Tasks;
using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    [AuthorizeRole("User", "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IMomoPaymentService _momoPaymentService;

        public OrderController(IOrderService orderService, ICartService cartService, IMomoPaymentService momoPaymentService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _momoPaymentService = momoPaymentService;
        }

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            var userId = GetCurrentUserId();
            var cart = _cartService.GetCartViewModel(userId);
            
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                FullName = Session["FullName"]?.ToString() ?? "",
                Phone = "",
                ShippingAddress = "",
                PaymentMethod = "COD",
                Notes = ""
            };

            ViewBag.Cart = cart;
            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var cart = _cartService.GetCartViewModel(userId);
                ViewBag.Cart = cart;
                return View(model);
            }
            
            var userId2 = GetCurrentUserId();
            var cart2 = _cartService.GetCartViewModel(userId2);
            
            if (cart2 == null || !cart2.Items.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi đặt hàng.");
                ViewBag.Cart = cart2;
                return View(model);
            }

            // Validate cart before creating order
            if (!_cartService.ValidateCart(userId2))
            {
                ModelState.AddModelError("", "Giỏ hàng có sản phẩm không hợp lệ hoặc đã hết hàng. Vui lòng kiểm tra lại giỏ hàng.");
                ViewBag.Cart = cart2;
                return View(model);
            }
            
            // Create order
            var order = new Order
            {
                UserId = userId2,
                OrderDate = System.DateTime.Now,
                ShippingAddress = model.ShippingAddress,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes,
                Status = "Pending"
            };
            
            var orderDetails = cart2.Items.Select(ci => new OrderDetail
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.FinalPrice,
                SubTotal = ci.SubTotal
            }).ToList();
            
            // Calculate shipping fee (free if total >= 500,000, otherwise 30,000)
            var subtotal = orderDetails.Sum(od => od.SubTotal);
            var shippingFee = subtotal >= 500000 ? 0 : 30000;
            order.ShippingFee = shippingFee;
            order.TotalAmount = subtotal + shippingFee;
            
            try
            {
                _orderService.CreateOrder(order, orderDetails);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Cart = cart2;
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo đơn hàng: " + ex.Message);
                ViewBag.Cart = cart2;
                return View(model);
            }
            
            if (model.PaymentMethod == "MoMo")
            {
                try
                {
                    var momoResponse = await _momoPaymentService.CreatePaymentAsync(
                        order.Id.ToString(),
                        order.TotalAmount,
                        $"Thanh toán đơn hàng #{order.Id}");

                    if (momoResponse == null || momoResponse.ResultCode != 0 || string.IsNullOrEmpty(momoResponse.PayUrl))
                    {
                        ModelState.AddModelError("", "Không tạo được liên kết thanh toán MoMo. Vui lòng thử lại hoặc chọn phương thức khác.");
                        ViewBag.Cart = cart2;
                        return View(model);
                    }

                    var momoVm = new MomoPaymentViewModel
                    {
                        OrderId = order.Id,
                        Amount = order.TotalAmount,
                        PayUrl = momoResponse.PayUrl,
                        QrCodeUrl = string.IsNullOrEmpty(momoResponse.QrCodeUrl) ? null : momoResponse.QrCodeUrl,
                        OrderInfo = $"Đơn hàng #{order.Id}"
                    };

                    // Không xóa giỏ ngay để tránh mất sản phẩm khi user chưa thanh toán
                    ViewBag.Cart = cart2;
                    ViewBag.MomoPayment = momoVm;
                    ViewBag.ShowMomoModal = true;
                    ModelState.Clear();
                    return View(model);
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", "Thanh toán MoMo gặp lỗi: " + ex.Message);
                    ViewBag.Cart = cart2;
                    return View(model);
                }
            }
            
            // Clear cart for COD
            _cartService.ClearCart(userId2);
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        // GET: Order/Confirmation
        public ActionResult Confirmation(int orderId)
        {
            var userId = GetCurrentUserId();
            var order = _orderService.GetById(orderId);
            
            if (order == null || order.UserId != userId)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/MyOrders
        public ActionResult MyOrders(string status = null)
        {
            var userId = GetCurrentUserId();
            var orders = _orderService.GetOrdersByUser(userId);
            
            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status != null && o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            
            ViewBag.CurrentStatus = status;
            return View(orders);
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            var userId = GetCurrentUserId();
            var order = _orderService.GetById(id);
            
            if (order == null || (order.UserId != userId && Session["Role"]?.ToString() != "Admin"))
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/CheckStatus/{orderId}
        [HttpGet]
        public JsonResult CheckStatus(int orderId)
        {
            var userId = GetCurrentUserId();
            var order = _orderService.GetById(orderId);
            
            if (order == null || order.UserId != userId)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new 
            { 
                success = true, 
                orderId = order.Id,
                status = order.Status,
                isPaid = order.Status == "Paid"
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Order/EditShippingAddress/5
        public ActionResult EditShippingAddress(int id)
        {
            var userId = GetCurrentUserId();
            var order = _orderService.GetById(id);
            
            if (order == null || order.UserId != userId)
            {
                return HttpNotFound();
            }

            if (order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Chỉ có thể sửa địa chỉ khi đơn hàng đang ở trạng thái Pending";
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.OrderId = order.Id;
            ViewBag.CurrentAddress = order.ShippingAddress;
            return View();
        }

        // POST: Order/UpdateShippingAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateShippingAddress(int orderId, string shippingAddress)
        {
            var userId = GetCurrentUserId();
            var order = _orderService.GetById(orderId);
            
            if (order == null || order.UserId != userId)
            {
                return HttpNotFound();
            }

            if (order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Chỉ có thể sửa địa chỉ khi đơn hàng đang ở trạng thái Pending";
                return RedirectToAction("Details", new { id = orderId });
            }

            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                TempData["ErrorMessage"] = "Địa chỉ giao hàng không được để trống";
                ViewBag.OrderId = orderId;
                ViewBag.CurrentAddress = order.ShippingAddress;
                return View("EditShippingAddress");
            }

            try
            {
                _orderService.UpdateShippingAddress(orderId, shippingAddress);
                TempData["SuccessMessage"] = "Đã cập nhật địa chỉ giao hàng thành công";
                return RedirectToAction("Details", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.OrderId = orderId;
                ViewBag.CurrentAddress = order.ShippingAddress;
                return View("EditShippingAddress");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật địa chỉ: " + ex.Message;
                ViewBag.OrderId = orderId;
                ViewBag.CurrentAddress = order.ShippingAddress;
                return View("EditShippingAddress");
            }
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var userId = GetCurrentUserId();
            var order = _orderService.GetById(id);
            
            if (order == null || order.UserId != userId)
            {
                return HttpNotFound();
            }

            try
            {
                _orderService.CancelOrder(id);
                TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công. Tồn kho sản phẩm đã được cập nhật.";
                return RedirectToAction("Details", new { id = id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy đơn hàng: " + ex.Message;
                return RedirectToAction("Details", new { id = id });
            }
        }

        private int GetCurrentUserId()
        {
            return Session["UserId"] != null ? (int)Session["UserId"] : 0;
        }
    }
}

