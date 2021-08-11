using Shop.Models;
using Shop.Repositories.Interfaces;
using System.Web.Mvc;

namespace Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderProductController : Controller
    {
        private readonly IUnitOfWork unit;

        public OrderProductController(IUnitOfWork unit)
        {
            this.unit = unit;
        }


        // Edit page
        // GET: Admin/OrderProduct/Edit
        public ActionResult Edit(int id)
        {
            OrderProduct orderProduct = unit.OrderProducts.Get(id);
            return PartialView("_EditOrderProduct", orderProduct);
        }

        [HttpPost]
        public ActionResult Edit(OrderProduct vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditOrderProduct", vm);
            }
            OrderProduct item = unit.OrderProducts.Get(vm.Id);

            item.Name = vm.Name;
            item.Count = vm.Count;
            item.Price = vm.Price;
            item.Subtotal = vm.Price * vm.Count;

            unit.OrderProducts.Update(item);
            unit.Complete();

            UpdateOrderTotal(item.OrderId);

            TempData["msg"] = "Order item updated successfully.";
            return RedirectToAction("Edit", "Orders", new { id = item.OrderId });
        }

        // ========================================================================
        // [DELETE] GET: Admin/Orders/Delete
        // ========================================================================
        public ActionResult Delete(int id, int orderId)
        {
            OrderProduct item = unit.OrderProducts.Get(id);

            if (item == null)
            {
                TempData["msg"] = "Error. Order item not found.";
            }
            else
            {
                unit.OrderProducts.Remove(item);
                unit.Complete();
                UpdateOrderTotal(orderId);
                TempData["msg"] = $"Order item: \"{item.Name}\" deleted successfully.";
            }

            return RedirectToAction("Edit", "Orders", new { id = orderId });
        }

        // ========================================================================
        // [INSERT] GET: Admin/Orders/Add
        // ========================================================================
        public ActionResult Add() 
        {
            return PartialView("_Add");
        }

        public ActionResult SearchProducts(int? page = null, string search = null)
        {
            ProductPage products = unit.Products.GetProducts(page: page, search: search);
            return PartialView("_SearchProducts", products);
        }

        // ========================================================================
        // [INSERT] Post: Admin/Orders/Add
        // ========================================================================
        [HttpPost]
        public ActionResult Add(int orderId, int productId)
        {
            OrderProduct dbOrderProduct = unit.OrderProducts.FirstOrDefault(p => p.OrderId == orderId && p.ProductId == productId);
            
            if (dbOrderProduct != null)
            {
                TempData["msg"] = "Product order already exists";
                return RedirectToAction("Edit", "Orders", new { id = orderId });
            }

            Product product = unit.Products.Get(productId);

            OrderProduct orderProduct = new OrderProduct
            {
                OrderId = orderId,
                Name = product.Name,
                ProductId = product.Id,
                Price = product.Price,
                Count = 1,
                CurrencyId = unit.Products.GetCurrency().Id,
                Subtotal = product.Price
            };

            unit.OrderProducts.Add(orderProduct);
            unit.Complete();

            // update order total
            var order = unit.Orders.Get(orderId);
            double total = 0;
            foreach (var item in order.Products)
            {
                total += item.Price;
            }
            order.TotalPrice = total;

            unit.Orders.Update(order);
            unit.Complete();

            UpdateOrderTotal(orderId);

            TempData["msg"] = "Product added successfully";
            return RedirectToAction("Edit", "Orders", new { id = orderId });
        }

        private void UpdateOrderTotal(int orderId)
        {
            var order = unit.Orders.Get(orderId);
            double total = 0;
            foreach (var item in order.Products)
            {
                total += item.Price * item.Count;
            }
            order.TotalPrice = total;

            unit.Orders.Update(order);
            unit.Complete();
        }
    }
}