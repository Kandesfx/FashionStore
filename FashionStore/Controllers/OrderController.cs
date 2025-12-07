using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
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

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
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
        public ActionResult Checkout(CheckoutViewModel model)
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
                return RedirectToAction("Index", "Cart");
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
            
            order.TotalAmount = orderDetails.Sum(od => od.SubTotal);
            
            _orderService.CreateOrder(order, orderDetails);
            
            // Clear cart
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
        public ActionResult MyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = _orderService.GetOrdersByUser(userId);
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

        private int GetCurrentUserId()
        {
            return Session["UserId"] != null ? (int)Session["UserId"] : 0;
        }
    }
}

