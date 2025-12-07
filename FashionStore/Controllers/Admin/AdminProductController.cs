using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public AdminProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: Admin/Product
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int totalCount;
            var products = _productService.GetProductsWithPaging(page, pageSize, out totalCount);
            
            ViewBag.Products = products;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            
            return View();
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.Categories = _categoryService.GetActiveCategories();
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetActiveCategories();
                return View(model);
            }

            try
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    DiscountPrice = model.DiscountPrice,
                    CategoryId = model.CategoryId,
                    ImageUrl = model.ImageUrl,
                    Stock = model.Stock,
                    Featured = model.Featured,
                    IsActive = model.IsActive,
                    CreatedDate = System.DateTime.Now
                };

                _productService.Add(product);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = _categoryService.GetActiveCategories();
                return View(model);
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                Featured = product.Featured,
                IsActive = product.IsActive
            };

            ViewBag.Categories = _categoryService.GetActiveCategories();
            return View(model);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetActiveCategories();
                return View(model);
            }

            try
            {
                var product = _productService.GetById(model.Id);
                if (product == null)
                {
                    return HttpNotFound();
                }

                product.ProductName = model.ProductName;
                product.Description = model.Description;
                product.Price = model.Price;
                product.DiscountPrice = model.DiscountPrice;
                product.CategoryId = model.CategoryId;
                product.ImageUrl = model.ImageUrl;
                product.Stock = model.Stock;
                product.Featured = model.Featured;
                product.IsActive = model.IsActive;

                _productService.Update(product);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = _categoryService.GetActiveCategories();
                return View(model);
            }
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _productService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var product = _productService.GetById(id);
                return View(product);
            }
        }
    }
}

