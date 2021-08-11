using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [MaxLength(50)]
        public string ShippingFirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShippingLastName { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ShippingCity { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShippingAddress { get; set; }

        [Required]
        [MaxLength(10)]
        public string ShippingPostCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string ShippingPhone { get; set; }

        [Required]
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}