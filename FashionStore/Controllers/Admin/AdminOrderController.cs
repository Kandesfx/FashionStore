using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Admin/Order
        public ActionResult Index(string status = null, int page = 1)
        {
            var orders = status != null 
                ? _orderService.GetOrdersByStatus(status).ToList()
                : _orderService.GetAll().OrderByDescending(o => o.OrderDate).ToList();

            int pageSize = 10;
            int totalCount = orders.Count;
            var pagedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Orders = pagedOrders;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);

            return View("~/Views/Admin/Order/Index.cshtml");
        }

        // GET: Admin/Order/Details/5
        public ActionResult Details(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/Admin/Order/Details.cshtml", order);
        }

        // POST: Admin/Order/UpdateStatus
        [HttpPost]
        public JsonResult UpdateStatus(int orderId, string status)
        {
            try
            {
                _orderService.UpdateOrderStatus(orderId, status);
                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

