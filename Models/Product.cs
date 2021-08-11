using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [MaxLength(100)]
        public string Thumbnail { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}