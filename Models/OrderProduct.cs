using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }
        
        [DisplayName("Quantity")]
        [Required]
        public int Count { get; set; }

        [Required]
        [DisplayName("Item price")]
        public double Price { get; set; }

        [Required]
        public double Subtotal { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual Order Order { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Product Product { get; set; }
    }
}