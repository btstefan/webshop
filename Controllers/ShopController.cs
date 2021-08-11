using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class ShopController : Controller
    {
        private readonly IUnitOfWork unit;

        public ShopController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // GET: Shop
        public ActionResult Index()
        {
            ViewBag.Categories = unit.Categories.GetAll();
            return View();
        }

        [Route("Shop/Products/{categoryId}")]
        public ActionResult Products(
            int categoryId,
            int? page = null,
            int? count = null,
            string orderBy = null,
            string search = null)
        {
            var category = unit.Categories.Get(categoryId);
            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Parents = unit.Categories.GetParentList(category);
            ViewBag.Category = category;

            ProductPage productPage = unit.Products.GetProducts(categoryId, page, count, orderBy, search);

            return View(productPage);
        }

        public ActionResult Search(
            string search = null,
            int? page = null,
            int? count = null,
            string orderBy = null)
        {
            ProductPage productPage = unit.Products.GetProducts(null, page, count, orderBy, search);
            return View(productPage);
        }

        public ActionResult Product(int id)
        {
            Product product = unit.Products.Get(id);
            if (product == null)
                return HttpNotFound();

            // if uncategorized
            if (product.CategoryId == null)
            {
                ViewBag.Parents = null;
                return View(product);
            }

            // if categpry not exists => set uncategorized attr
            Category category = unit.Categories.Get((int)product.CategoryId);
            if (category == null)
            {
                ViewBag.Parents = null;
            }
            else
            {
                ViewBag.Parents = unit.Categories.GetParentList(category);
            }

            return View(product);
        }
        
    }
}