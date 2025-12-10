using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminInventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;

        public AdminInventoryController(IInventoryService inventoryService, IProductService productService)
        {
            _inventoryService = inventoryService;
            _productService = productService;
        }

        // GET: Admin/Inventory
        public ActionResult Index(int? productId, string transactionType = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1)
        {
            int pageSize = 20;
            
            var transactions = _inventoryService.GetTransactions(productId, transactionType, startDate, endDate).ToList();
            
            int totalCount = transactions.Count;
            var pagedTransactions = transactions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Transactions = pagedTransactions;
            ViewBag.Products = _productService.GetAll().ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.ProductId = productId;
            ViewBag.TransactionType = transactionType;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.TransactionTypes = new[] { "Import", "Export", "Sale", "Return", "Adjustment_Increase", "Adjustment_Decrease" };

            return View("~/Views/Admin/Inventory/Index.cshtml");
        }

        // GET: Admin/Inventory/LowStock
        public ActionResult LowStock(int? threshold = null)
        {
            int stockThreshold = threshold ?? 10;
            var lowStockProducts = _inventoryService.GetLowStockProducts(stockThreshold).ToList();
            
            ViewBag.LowStockProducts = lowStockProducts;
            ViewBag.LowStockCount = lowStockProducts.Count;
            ViewBag.Threshold = stockThreshold;

            return View("~/Views/Admin/Inventory/LowStock.cshtml");
        }

        // GET: Admin/Inventory/Import
        public ActionResult Import(int? productId = null)
        {
            ViewBag.Products = _productService.GetAll().ToList();
            ViewBag.SelectedProductId = productId;
            if (productId.HasValue)
            {
                var product = _productService.GetById(productId.Value);
                if (product != null)
                {
                    ViewBag.CurrentStock = _inventoryService.GetCurrentStock(productId.Value);
                }
            }
            return View("~/Views/Admin/Inventory/Import.cshtml");
        }

        // POST: Admin/Inventory/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(int productId, int quantity, string notes)
        {
            try
            {
                int userId = GetCurrentUserId();
                _inventoryService.ImportStock(productId, quantity, notes, userId);
                TempData["SuccessMessage"] = $"Đã nhập {quantity} sản phẩm vào kho thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi nhập kho: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Inventory/Export
        public ActionResult Export()
        {
            ViewBag.Products = _productService.GetAll().ToList();
            return View("~/Views/Admin/Inventory/Export.cshtml");
        }

        // POST: Admin/Inventory/Export
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(int productId, int quantity, string notes)
        {
            try
            {
                int userId = GetCurrentUserId();
                _inventoryService.ExportStock(productId, quantity, notes, userId);
                TempData["SuccessMessage"] = $"Đã xuất {quantity} sản phẩm khỏi kho thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xuất kho: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Inventory/Adjust
        public ActionResult Adjust()
        {
            ViewBag.Products = _productService.GetAll().ToList();
            return View("~/Views/Admin/Inventory/Adjust.cshtml");
        }

        // POST: Admin/Inventory/Adjust
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Adjust(int productId, int quantity, string reason)
        {
            try
            {
                int userId = GetCurrentUserId();
                _inventoryService.AdjustStock(productId, quantity, reason, userId);
                string action = quantity > 0 ? "tăng" : "giảm";
                TempData["SuccessMessage"] = $"Đã điều chỉnh tồn kho {action} {Math.Abs(quantity)} sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi điều chỉnh tồn kho: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/Inventory/Product/{productId}
        public ActionResult Product(int productId)
        {
            var product = _productService.GetById(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var transactions = _inventoryService.GetTransactions(productId).ToList();
            var currentStock = _inventoryService.GetCurrentStock(productId);
            var summary = _inventoryService.GetStockMovementsSummary(productId);

            ViewBag.Product = product;
            ViewBag.Transactions = transactions;
            ViewBag.CurrentStock = currentStock;
            ViewBag.Summary = summary;

            return View("~/Views/Admin/Inventory/Product.cshtml");
        }

        // GET: Admin/Inventory/GetProductStock
        [HttpGet]
        public JsonResult GetProductStock(int productId)
        {
            try
            {
                var product = _productService.GetById(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" }, JsonRequestBehavior.AllowGet);
                }

                var currentStock = _inventoryService.GetCurrentStock(productId);
                return Json(new { success = true, stock = currentStock, productName = product.ProductName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            throw new UnauthorizedAccessException("User not authenticated");
        }
    }
}

