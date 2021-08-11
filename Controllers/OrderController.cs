using Shop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Shop.Repositories.Interfaces;
using Shop.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unit;

        public OrderController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // GET: Order
        public ActionResult Index()
        {
            AddOrderViewModel orderVM = new AddOrderViewModel();
            //ModelState.AddModelError("ShippingPhone", "neki error");

            List<CartItem> cartItems = (List<CartItem>)Session["cart"];
            if (cartItems == null || cartItems.Count == 0)
                return RedirectToAction("Index", "Home");

            if (TempData["Validation"] == null)
            {
                // if user logged in --> fill form
                if (User.Identity.IsAuthenticated)
                {
                    var x = User.Identity.GetUserId();
                    ApplicationUser user = unit.Users.FirstOrDefault(u => u.Id == x);
                    if (user != null)
                    {
                        orderVM.ShippingCity = user.City;
                        orderVM.ShippingAddress = user.Address;
                        orderVM.ShippingFirstName = user.FirstName;
                        orderVM.ShippingLastName = user.LastName;
                        orderVM.ShippingPhone = user.PhoneNumber;
                        orderVM.ShippingPostCode = user.PostCode;
                    }
                }
            }
            else
            {
                ViewData = (ViewDataDictionary)TempData["Validation"];
            }

            // get active payment types for PaymentTypeId dropdown
            ViewBag.Payments = unit.Orders.GetPaymentTypes(true).ToList();

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(AddOrderViewModel orderVM)
        {
            try
            {
                List<CartItem> cartItems = (List<CartItem>)Session["cart"];
                if (cartItems == null || cartItems.Count == 0)
                    throw new ValidationException("Cart is empty");

                if (!ModelState.IsValid)
                {
                    TempData["Validation"] = ViewData;
                    return RedirectToAction("Index");
                }

                // create order
                Order newOrder = new Order
                {
                    PaymentTypeId = orderVM.PaymentTypeId,
                    OrderStatusId = 1,
                    ShippingFirstName = orderVM.ShippingFirstName,
                    ShippingLastName = orderVM.ShippingLastName,
                    ShippingCity = orderVM.ShippingCity,
                    ShippingAddress = orderVM.ShippingAddress,
                    ShippingPostCode = orderVM.ShippingPostCode,
                    ShippingPhone = orderVM.ShippingPhone,
                    CurrencyId = unit.Products.GetCurrency().Id,
                    TotalPrice = unit.Orders.GetTotal(cartItems),
                    Created = DateTime.Now
                };

                if (User.Identity.IsAuthenticated)
                    newOrder.UserId = User.Identity.GetUserId();

                unit.Orders.Add(newOrder);
                unit.Complete();

                // add products to order
                List<OrderProduct> orderProducts = new List<OrderProduct>();
                foreach (CartItem item in cartItems)
                {
                    orderProducts.Add(new OrderProduct
                    {
                        OrderId = newOrder.Id,
                        ProductId = item.Product.Id,
                        Count = item.Quantity,
                        Price = item.Product.Price,
                        Subtotal = item.Product.Price * item.Quantity,
                        CurrencyId = item.Product.CurrencyId,
                        Name = item.Product.Name
                    });
                }

                unit.Orders.AddProducts(orderProducts);
                unit.Complete();

                // empty cart
                Session["cart"] = null;

                return RedirectToAction("OrderCreated", new { id = newOrder.Id });
            }
            catch (Exception ex)
            {
                unit.Dispose();
                TempData["error"] = ex.Message;
                return View("OrderFailed");
            }
        }

        public ActionResult OrderCreated(int id)
        {
            ViewBag.Currency = unit.Products.GetCurrency();
            Order order = unit.Orders.Get(id);

            bool isAuthorized = false;
            if (order.UserId != null && order.OrderStatusId == 1 && User.Identity.GetUserId() == order.UserId)
            {
                isAuthorized = true;
            }

            ViewBag.IsAuthorized = isAuthorized;
            return View(order);
        }

        [Authorize]
        public ActionResult CancelOrder(int id)
        {
            string userId = User.Identity.GetUserId();

            Order order = unit.Orders.Get(id);

            if (order.UserId != userId)
                return RedirectToAction("OrderCreated", new { id });

            order.OrderStatusId = 3;

            unit.Orders.Update(order);
            unit.Complete();

            return RedirectToAction("OrderCreated", new { id });
        }

    }
}