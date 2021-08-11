using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class CategoryTree
    {
        public Category Category { get; set; }
        public List<CategoryTree> SubCategories { get; set; }
    }
}