using Shop.Models;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork unit;

        public CartController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // GET: Cart
        public ActionResult Index()
        {
            List<CartItem> cartItems = (List<CartItem>)Session["cart"];
            double total = unit.Orders.GetTotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.Total = total;
            ViewBag.Currency = unit.Products.GetCurrency();

            return View();
        }

        public PartialViewResult CartMenu()
        {
            List<CartItem> cartItems = (List<CartItem>)Session["cart"];
            if (cartItems != null)
                ViewBag.ItemCount = cartItems.Count;
            else
                ViewBag.ItemCount = 0;

            return PartialView("_CartMenu");
        }

        public JsonResult AddToCart(int id, int qnt = 1)
        {
            Product product = unit.Products.Get(id);

            if (qnt <= 0)
                return Json(new { Success = false, Message = "Negative count." });

            if (Session["cart"] == null)
            {
                List<CartItem> cart = new List<CartItem>() {
                    new CartItem
                    {
                        Product = product,
                        Quantity = qnt
                    }
                };
                Session["cart"] = cart;
            }
            else
            {
                List<CartItem> cart = (List<CartItem>)Session["cart"];
                int x = IfExist(id);
                if (x != -1)
                {
                    return Json(new { Success = false, Message = "Product is already in your cart." });
                }

                cart.Add(new CartItem { Product = product, Quantity = qnt });
                Session["cart"] = cart;
            }
            return Json(new { Success = true, Message = "Product is added successfully." });
        }

        [HttpPost]
        public JsonResult Update(int id, int qnt)
        {
            List<CartItem> cartItems = (List<CartItem>)Session["cart"];

            // validate quantity
            if (qnt <= 0)
                return Json(new { Success = false, Message = "Negative count." });

            // if list is empty
            if (cartItems == null || cartItems.Count == 0)
                return Json(new { Success = false, Message = "Cart is empty." });

            // if product not exists
            int index = IfExist(id);
            if (index < 0)
                return Json(new { Success = false, Message = "Product not found." });

            // update
            //cartItems.Where(i => i.Product.Id == id).ToList().ForEach(c => c.Quantity = qnt);
            cartItems[index].Quantity = qnt;

            Session["cart"] = cartItems;

            return Json(new { Success = true, Message = "Cart edited." });
        }

        public ActionResult Remove(int id)
        {
            List<CartItem> cart = (List<CartItem>)Session["cart"];
            int index = IfExist(id);
            if (index >= 0)
            {
                cart.RemoveAt(index);
                Session["cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        private int IfExist(int id)
        {
            List<CartItem> cart = (List<CartItem>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.Id.Equals(id))
                    return i;
            return -1;
        }
    }
}