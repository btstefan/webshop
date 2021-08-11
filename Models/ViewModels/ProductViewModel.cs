using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Shop.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        [DisplayName("New thumbnail")]
        public HttpPostedFileBase Thumbnail { get; set; }

        [DisplayName("New images")]
        public List<HttpPostedFileBase> Images { get; set; }

        [DisplayName("Current images")]
        public IEnumerable<int> ActiveImages { get; set; }
    }
}