using Accord.Collections;
using Shop.Models;
using Shop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Repositories.Interfaces
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<Category> GetSubCategories(int categoryId)
        {
            return Context.Categories.Where(c => c.ParentCategoryId == categoryId);
        }

        public List<CategoryTree> GetCategoryTree(int? id = null)
        {
            List<Category> categories = Context.Categories.ToList();
            List<CategoryTree> _categoryTree = new List<CategoryTree>();

            foreach (Category c in categories)
                _categoryTree.Add(new CategoryTree { Category = c });

            List<CategoryTree> categoryTreeList = GetMenuTree(_categoryTree, id);
            return categoryTreeList;
        }

        private List<CategoryTree> GetMenuTree(List<CategoryTree> list, int? categoryId)
        {
            return list.Where(x => x.Category.ParentCategoryId == categoryId).Select(x => new CategoryTree()
            {
                Category = x.Category,
                SubCategories = GetMenuTree(list, x.Category.Id)
            }).ToList();
        }

        public List<Category> GetParentList(Category category)
        {
            List<Category> parents = new List<Category>();
            
            if (category.ParentCategoryId == null)
                return parents;

            Category parent = category;

            do
            {
                // find
                var id = parent.ParentCategoryId;
                parent = Context.Categories.Find(id);
                // add
                parents.Add(parent);
            }
            while (parent.ParentCategoryId != null);

            parents.Reverse();

            return parents;
        }
    }
}