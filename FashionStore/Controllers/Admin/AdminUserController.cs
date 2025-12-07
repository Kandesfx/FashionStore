using System;
using System.Linq;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.Entities;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers.Admin
{
    [AuthorizeRole("Admin")]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;
        private readonly FashionStore.Repositories.Interfaces.IRoleRepository _roleRepository;

        public AdminUserController(IUserService userService, FashionStore.Repositories.Interfaces.IRoleRepository roleRepository)
        {
            _userService = userService;
            _roleRepository = roleRepository;
        }

        // GET: Admin/User
        public ActionResult Index(int page = 1)
        {
            var users = _userService.GetAll().OrderByDescending(u => u.CreatedDate).ToList();
            
            int pageSize = 10;
            int totalCount = users.Count;
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Users = pagedUsers;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);

            return View();
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/User/Edit/5
        public ActionResult Edit(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Roles = _roleRepository.GetAll();
            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                var existingUser = _userService.GetById(user.Id);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                existingUser.FullName = user.FullName;
                existingUser.Phone = user.Phone;
                existingUser.Address = user.Address;
                existingUser.Email = user.Email;
                existingUser.RoleId = user.RoleId;
                existingUser.IsActive = user.IsActive;

                _userService.Add(existingUser);
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }

        // GET: Admin/User/Delete/5
        public ActionResult Delete(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var user = _userService.GetById(id);
                if (user != null)
                {
                    user.IsActive = false;
                    _userService.Add(user);
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var user = _userService.GetById(id);
                return View(user);
            }
        }
    }
}

