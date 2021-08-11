using Shop.Models;
using Shop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        IEnumerable<Category> GetSubCategories(int categoryId);
        List<CategoryTree> GetCategoryTree(int? id = null);
        List<Category> GetParentList(Category category);
    }
}
