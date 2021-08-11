using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unit;
        //private ApplicationUserManager _userManager;
        public HomeController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // GET: Admin/Home
        public ActionResult Index()
        {
            AdminViewModel adminViewModel = new AdminViewModel()
            {
                ProductCount = unit.Products.Count(),
                UsersCount = unit.Users.Count(),
                CategoryCount = unit.Categories.Count(),
                CategoryActiveCount = unit.Categories.Count(c => c.Active),
                OrderCount = unit.Orders.Count(),
                ActiveOrders = unit.Orders.Count(o => o.OrderStatusId == 1)
            };

            return View(adminViewModel);
        }
    }
}