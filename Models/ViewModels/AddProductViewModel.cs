using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models.ViewModels
{
    public class AddProductViewModel
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { get; set; }

        [Required]
        public double Price { get; set; }

        public string Description { get; set; }

        [Required]
        public HttpPostedFileBase Thumbnail { get; set; }

        public List<HttpPostedFileBase> Images { get; set; }
    }
}