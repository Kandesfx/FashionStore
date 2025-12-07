using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public AdminDashboardController(IOrderService orderService, IProductService productService, IUserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            var statistics = _orderService.GetOrderStatistics();
            var recentOrders = _orderService.GetOrdersByStatus("Pending").Take(10);
            
            ViewBag.Statistics = statistics;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.TotalProducts = _productService.GetActiveProducts().Count();
            ViewBag.TotalUsers = _userService.GetAll().Count();
            
            return View();
        }
    }
}

