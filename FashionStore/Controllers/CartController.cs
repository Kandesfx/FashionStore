using System;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // GET: Cart
        public ActionResult Index()
        {
            var userId = GetCurrentUserId();
            var cartViewModel = _cartService.GetCartViewModel(userId);
            
            // Pass to ViewBag for compatibility with existing view
            ViewBag.CartItems = cartViewModel.Items;
            ViewBag.TotalAmount = cartViewModel.TotalAmount;
            ViewBag.TotalItems = cartViewModel.TotalItems;
            ViewBag.CartId = cartViewModel.CartId;
            
            return View(cartViewModel);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng." });
                }
                
                _cartService.AddToCart(userId, productId, quantity);
                
                var cartCount = _cartService.GetCartItemCount(userId);
                return Json(new { success = true, message = "Đã thêm vào giỏ hàng", cartCount = cartCount });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Cart/UpdateCartItem
        [HttpPost]
        public JsonResult UpdateCartItem(int cartItemId, int quantity)
        {
            try
            {
                _cartService.UpdateCartItem(cartItemId, quantity);
                var userId = GetCurrentUserId();
                var cartViewModel = _cartService.GetCartViewModel(userId);
                return Json(new { success = true, totalAmount = cartViewModel.TotalAmount });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Cart/RemoveCartItem
        [HttpPost]
        public JsonResult RemoveCartItem(int cartItemId)
        {
            try
            {
                _cartService.RemoveCartItem(cartItemId);
                var userId = GetCurrentUserId();
                var cartViewModel = _cartService.GetCartViewModel(userId);
                return Json(new { success = true, totalAmount = cartViewModel.TotalAmount, cartCount = cartViewModel.TotalItems });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Cart/GetCartCount
        [HttpGet]
        public JsonResult GetCartCount()
        {
            var userId = GetCurrentUserId();
            var count = _cartService.GetCartItemCount(userId);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }

        private int GetCurrentUserId()
        {
            return Session["UserId"] != null ? (int)Session["UserId"] : 0;
        }
    }
}

