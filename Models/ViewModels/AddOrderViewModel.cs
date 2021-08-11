using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models.ViewModels
{
    public class AddOrderViewModel
    {
        [DisplayName("First name")]
        [Required(ErrorMessage = "Please enter your first name.")]
        [StringLength(50, ErrorMessage = "The First Name must be less than {1} characters.")]
        public string ShippingFirstName { get; set; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Please enter your last name.")]
        [StringLength(50, ErrorMessage = "The Last Name must be less than {1} characters.")]
        public string ShippingLastName { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "Please enter your city.")]
        [StringLength(100, ErrorMessage = "The City Name must be less than {1} characters.")]
        public string ShippingCity { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Please enter your Address.")]
        [StringLength(100, ErrorMessage = "The Address must be less than {1} characters.")]
        public string ShippingAddress { get; set; }

        [DisplayName("Post code")]
        [Required(ErrorMessage = "Please enter your Post Code.")]
        [StringLength(10, ErrorMessage = "The Post Code must have less than {1} characters.")]
        public string ShippingPostCode { get; set; }

        [DisplayName("Phone number")]
        [Required(ErrorMessage = "Please enter your Phone number.")]
        [StringLength(20, ErrorMessage = "The Phone number be less than {1} characters.")]
        public string ShippingPhone { get; set; }

        [DisplayName("Payment type")]
        [Required(ErrorMessage = "Please select Payment type.")]
        public int PaymentTypeId { get; set; }
    }
}