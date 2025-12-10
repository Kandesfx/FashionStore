using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.Entities;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminPromotionController : Controller
    {
        private readonly IPromotionService _promotionService;

        public AdminPromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        // GET: Admin/Promotion
        public ActionResult Index(int page = 1)
        {
            int pageSize = 20;
            var promotions = _promotionService.GetAll().ToList();
            
            int totalCount = promotions.Count;
            var pagedPromotions = promotions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Promotions = pagedPromotions;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View("~/Views/Admin/Promotion/Index.cshtml");
        }

        // GET: Admin/Promotion/Create
        public ActionResult Create()
        {
            return View("~/Views/Admin/Promotion/Create.cshtml");
        }

        // POST: Admin/Promotion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Promotion promotion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _promotionService.Create(promotion);
                    TempData["SuccessMessage"] = "Khuyến mãi đã được tạo thành công.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo khuyến mãi: " + ex.Message;
            }

            return View("~/Views/Admin/Promotion/Create.cshtml", promotion);
        }

        // GET: Admin/Promotion/Edit/5
        public ActionResult Edit(int id)
        {
            var promotion = _promotionService.GetById(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/Admin/Promotion/Edit.cshtml", promotion);
        }

        // POST: Admin/Promotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Promotion promotion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _promotionService.Update(promotion);
                    TempData["SuccessMessage"] = "Khuyến mãi đã được cập nhật thành công.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật khuyến mãi: " + ex.Message;
            }

            return View("~/Views/Admin/Promotion/Edit.cshtml", promotion);
        }

        // GET: Admin/Promotion/Details/5
        public ActionResult Details(int id)
        {
            var promotion = _promotionService.GetById(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }

            ViewBag.Promotion = promotion;
            ViewBag.IsValid = _promotionService.IsPromotionValid(id);

            return View("~/Views/Admin/Promotion/Details.cshtml");
        }

        // POST: Admin/Promotion/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _promotionService.Delete(id);
                TempData["SuccessMessage"] = "Khuyến mãi đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa khuyến mãi: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}

