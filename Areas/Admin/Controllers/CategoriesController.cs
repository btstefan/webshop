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
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork unit;

        public CategoriesController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // GET: Admin/Category
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult CategoryUpdateForm(int id)
        {
            Category category = unit.Categories.Get(id);

            List<CategoryTree> categoryTree = unit.Categories.GetCategoryTree();
            ViewBag.CategoryTree = categoryTree;
            return PartialView("EditCategoryForm", category);
        }
    }
}