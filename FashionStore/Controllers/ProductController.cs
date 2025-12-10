using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductVariantService _productVariantService;

        public ProductController(IProductService productService, ICategoryService categoryService, IProductVariantService productVariantService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productVariantService = productVariantService;
        }

        // GET: Product
        public ActionResult Index(int? categoryId, string sortBy, int page = 1)
        {
            var products = _productService.GetActiveProducts().ToList();
            
            // Filter by category
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            
            // Sorting
            switch (sortBy)
            {
                case "price-asc":
                    products = products.OrderBy(p => p.FinalPrice).ToList();
                    break;
                case "price-desc":
                    products = products.OrderByDescending(p => p.FinalPrice).ToList();
                    break;
                case "name-asc":
                    products = products.OrderBy(p => p.ProductName).ToList();
                    break;
                case "name-desc":
                    products = products.OrderByDescending(p => p.ProductName).ToList();
                    break;
                default:
                    products = products.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
            }
            
            // Pagination
            int pageSize = 12;
            int totalCount = products.Count;
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            ViewBag.Categories = _categoryService.GetActiveCategories();
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Products = pagedProducts;
            
            return View();
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            var product = _productService.GetById(id);
            if (product == null || !product.IsActive)
            {
                return HttpNotFound();
            }

            // Get product variants (size, color)
            var variants = _productVariantService.GetByProductId(id).Where(v => v.IsActive).ToList();
            
            // Create anonymous objects for JSON serialization (avoid circular reference)
            var variantsForJson = variants.Select(v => new
            {
                Id = v.Id,
                Size = v.Size,
                Color = v.Color,
                SKU = v.SKU,
                Price = v.Price,
                Stock = v.Stock,
                ImageUrl = v.ImageUrl,
                IsActive = v.IsActive
            }).ToList();
            
            // Get unique sizes and colors
            var sizes = variants.Where(v => !string.IsNullOrEmpty(v.Size))
                               .Select(v => v.Size)
                               .Distinct()
                               .OrderBy(s => s)
                               .ToList();
            
            var colors = variants.Where(v => !string.IsNullOrEmpty(v.Color))
                                .Select(v => v.Color)
                                .Distinct()
                                .OrderBy(c => c)
                                .ToList();

            // Get related products (same category)
            var relatedProducts = _productService.GetProductsByCategory(product.CategoryId)
                .Where(p => p.Id != id)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.Variants = variantsForJson; // Use anonymous objects instead of entities
            ViewBag.Sizes = sizes;
            ViewBag.Colors = colors;
            return View(product);
        }

        // GET: Product/Search
        public ActionResult Search(string q, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            var products = _productService.SearchProducts(q).ToList();
            
            int pageSize = 12;
            int totalCount = products.Count;
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            ViewBag.SearchTerm = q;
            ViewBag.Products = pagedProducts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            
            return View();
        }
    }
}

