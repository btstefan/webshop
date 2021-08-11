using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models.ViewModels
{
    public class AddCategoryViewModel
    {
        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; }

        [DisplayName("Parent Category")]
        public int? ParentCategoryId { get; set; }

        [Required]
        [DisplayName("Status")]
        public bool Active { get; set; }
    }
}