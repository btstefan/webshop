using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Parent Category")]
        public int? ParentCategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Status")]
        public bool Active { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        [JsonIgnore]
        public virtual Category ParentCategory { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}