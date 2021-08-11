using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class ProductPage
    {
        public List<Product> Products { get; set; }
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public int PageSize { get; set; }
    }
}