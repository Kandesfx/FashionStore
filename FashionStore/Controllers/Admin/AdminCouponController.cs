using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.Entities;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminCouponController : Controller
    {
        private readonly ICouponService _couponService;

        public AdminCouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // GET: Admin/Coupon
        public ActionResult Index(int page = 1)
        {
            int pageSize = 20;
            var coupons = _couponService.GetAll().ToList();
            
            int totalCount = coupons.Count;
            var pagedCoupons = coupons.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Coupons = pagedCoupons;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View("~/Views/Admin/Coupon/Index.cshtml");
        }

        // GET: Admin/Coupon/Create
        public ActionResult Create()
        {
            return View("~/Views/Admin/Coupon/Create.cshtml");
        }

        // POST: Admin/Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Coupon coupon)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _couponService.Create(coupon);
                    TempData["SuccessMessage"] = "Mã giảm giá đã được tạo thành công.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo mã giảm giá: " + ex.Message;
            }

            return View("~/Views/Admin/Coupon/Create.cshtml", coupon);
        }

        // GET: Admin/Coupon/Edit/5
        public ActionResult Edit(int id)
        {
            var coupon = _couponService.GetById(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/Admin/Coupon/Edit.cshtml", coupon);
        }

        // POST: Admin/Coupon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Coupon coupon)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _couponService.Update(coupon);
                    TempData["SuccessMessage"] = "Mã giảm giá đã được cập nhật thành công.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật mã giảm giá: " + ex.Message;
            }

            return View("~/Views/Admin/Coupon/Edit.cshtml", coupon);
        }

        // GET: Admin/Coupon/Details/5
        public ActionResult Details(int id)
        {
            var coupon = _couponService.GetById(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }

            var usages = _couponService.GetCouponUsages(id).ToList();

            ViewBag.Coupon = coupon;
            ViewBag.Usages = usages;
            ViewBag.UsageCount = _couponService.GetUsageCount(id);
            ViewBag.IsValid = _couponService.IsCouponValid(coupon.Code);

            return View("~/Views/Admin/Coupon/Details.cshtml");
        }

        // POST: Admin/Coupon/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _couponService.Delete(id);
                TempData["SuccessMessage"] = "Mã giảm giá đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa mã giảm giá: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}

