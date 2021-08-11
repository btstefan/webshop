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
    public class CurrencyController : Controller
    {
        private readonly IUnitOfWork unit;

        public CurrencyController(IUnitOfWork unit)
        {
            this.unit = unit;
        }


        // GET: Admin/Currency
        public ActionResult Index()
        {
            Currency currency = unit.Products.GetCurrency();
            return View(currency);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Currency currency)
        {
            if (!ModelState.IsValid)
                return View();

            unit.Products.UpdateCurrency(currency);
            unit.Complete();
            TempData["msg"] = "Currency settings are saved successfully";
            return RedirectToAction("Index");
        }
    }
}