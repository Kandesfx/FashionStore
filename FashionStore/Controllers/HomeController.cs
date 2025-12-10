using System.Web.Mvc;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var featuredProducts = _productService.GetFeaturedProducts(8);
            var latestProducts = _productService.GetLatestProducts(8);
            var categories = _categoryService.GetActiveCategories();
            
            ViewBag.FeaturedProducts = featuredProducts;
            ViewBag.LatestProducts = latestProducts;
            ViewBag.Categories = categories;
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

