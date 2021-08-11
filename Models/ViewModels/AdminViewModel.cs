using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Models.ViewModels
{
    public class AdminViewModel
    {
        public int ProductCount { get; set; }
        public int UsersCount { get; set; }
        public int CategoryCount { get; set; }
        public int CategoryActiveCount { get; set; }
        public int OrderCount { get; set; }
        public int ActiveOrders { get; set; }
    }
}