using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Shop.Areas.Admin.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : ApiController
    {
        private readonly IUnitOfWork _db;
        //private ApplicationUserManager _userManager;


        public CategoryController(IUnitOfWork db)
        {
            _db = db;
        }

        // GET: api/Category/GetTree
        public List<CategoryTree> GetTree()
        {
            List<CategoryTree> tree = _db.Categories.GetCategoryTree();
            return tree;
        }

        // GET: api/Category/GetTree/id
        public List<CategoryTree> GetTree(int id)
        {
            List<CategoryTree> tree = _db.Categories.GetCategoryTree(id);
            return tree;
        }

        // POST: api/Category/Update
        [HttpPost]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Update([FromBody]Category c)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { Success = false, Message = $"Form data is not valid." });

                c.Modified = DateTime.Now;
                _db.Categories.Update(c);
                _db.Complete();

                return Ok(new { Success = true, Message = $"Category \"{c.Name}\" is updated successfuly." });
            }
            catch(Exception)
            {
                return Ok(new { Success = false, Message = $"Unable to update category. Try again later" });
            }
        }

        // PUT: api/Category/Insert
        [HttpPut]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Insert([FromBody]AddCategoryViewModel c)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new { Success = false, Message = $"Form data is not valid." });

                Category newCategory = new Category
                {
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId,
                    Active = c.Active,
                    Created = DateTime.Now
                };

                _db.Categories.Add(newCategory);
                _db.Complete();
                return Ok(new { Success = true, Message = $"Category \"{c.Name}\" is added successfully." });
            }
            catch (Exception)
            {
                return Ok(new { Success = false, Message = $"Unable to add category. Try again later." });
            }
        }

        // DELETE: api/Category/5
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                Category category = _db.Categories.Get(id);

                if (category is null)
                    return Ok(new { Success = false, Message = $"Category doesn't exist." });

                //Expression<Func<Category, bool>> childExp = c => c.ParentCategoryId == id;
                if (_db.Categories.FirstOrDefault(c => c.ParentCategoryId == id) != null)
                    return Ok(new { Success = false, Message = $"You need to delete subcategories first." });

                _db.Categories.Remove(category);
                _db.Complete();
                return Ok(new { Success = true, Message = $"Category {category.Name} is permanently deleted." });
            }
            catch (Exception)
            {
                return Ok(new { Success = false, Message = $"Unable to delete category. Try again later." });
            }
        }
    }
}
