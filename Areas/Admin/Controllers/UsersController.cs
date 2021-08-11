using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shop.Models;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork unit;

        public UsersController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // ========================================================================
        // [READ] GET: Admin/Users
        // ========================================================================
        public ActionResult Index()
        {
            IEnumerable<ApplicationUser> users = unit.Users.GetAll();
            return View(users);
        }

        // ========================================================================
        // [DELETE] GET: Admin/Users/Delete/{id}
        // ========================================================================
        public ActionResult Delete(string id)
        {
            ApplicationUser user = unit.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                TempData["msg"] = "User not found";
                return RedirectToAction("Index");
            }
            unit.Users.Remove(user);
            unit.Complete();

            TempData["msg"] = "User deleted";
            return RedirectToAction("Index");
        }

        // ========================================================================
        // [EDIT] GET: Admin/Users/Edit/{id}
        // ========================================================================
        public ActionResult Edit(string id, string email)
        {
            ApplicationUser user = null;

            if (id != null)
                user = unit.Users.FirstOrDefault(u => u.Id == id);
            else if (email != null)
                user = unit.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                TempData["msg"] = "User not found";
                return RedirectToAction("Index");
            }
            
            return View(user);
        }

        // ========================================================================
        // [POST] GET: Admin/Users/Edit/
        // ========================================================================
        [HttpPost]
        public ActionResult Edit(ApplicationUser vm)
        {
            ApplicationUser user = unit.Users.FirstOrDefault(u => u.Id == vm.Id);

            if (user == null)
            {
                TempData["msg"] = "User not found.";
                return RedirectToAction("Index");
            }

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Address = vm.Address;
            user.PostCode = vm.PostCode;
            user.City = vm.City;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;

            unit.Users.Update(user);
            unit.Complete();

            TempData["msg"] = "User edited.";
            return RedirectToAction("Index");

        }

    }
}