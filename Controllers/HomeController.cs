using Microsoft.AspNet.Identity;
using Shop.Models;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unit;
        public HomeController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        public ActionResult Index()
        {
            List<Product> products = unit.Products.GetNewest(10).ToList();
            return View(products);
        }

        public PartialViewResult CategoryPartial()
        {
            var tree = unit.Categories.GetCategoryTree();
            return PartialView("_CategoryMenu", tree);
        }

        // user profile (auth access only)
        public ActionResult MyProfile()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ProfileViewModel userVM = new ProfileViewModel();
            

            if (TempData["Validation"] == null)
            {
                string userId = User.Identity.GetUserId();
                ApplicationUser user = unit.Users.FirstOrDefault(u => u.Id == userId);

                //fill form
                userVM.FirstName = user.FirstName;
                userVM.LastName = user.LastName;
                userVM.Address = user.Address;
                userVM.PostCode = user.PostCode;
                userVM.City = user.City;
                userVM.PhoneNumber = user.PhoneNumber;

                ViewBag.Orders = user.Orders.OrderByDescending(o => o.Created).ToList();
            }
            else
            {
                ViewData = (ViewDataDictionary)TempData["Validation"];
            }

            return View(userVM);
        }

        public ActionResult UpdateAccount(ProfileViewModel vm)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                TempData["Validation"] = ViewData;
                return RedirectToAction("MyProfile");
            }

            // get current user
            string userId = User.Identity.GetUserId();
            ApplicationUser user = unit.Users.FirstOrDefault(u => u.Id == userId);

            // update from form
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Address = vm.Address;
            user.PostCode = vm.PostCode;
            user.City = vm.City;
            user.PhoneNumber = vm.PhoneNumber;

            unit.Users.Update(user);
            unit.Complete();
            return RedirectToAction("MyProfile");
        }
    }
}