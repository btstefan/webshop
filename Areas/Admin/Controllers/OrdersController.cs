using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork unit;

        public OrdersController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // ========================================================================
        // [READ] GET: Admin/Orders
        // ========================================================================
        public ActionResult Index()
        {
            IEnumerable<Order> orders = unit.Orders.GetAll().OrderByDescending(o => o.Created);
            return View(orders.ToList());
        }

        // ========================================================================
        // [READ] GET: Admin/Orders/Details/{id}
        // ========================================================================
        public ActionResult Details(int id)
        {
            Order order = unit.Orders.Get(id);
            ViewBag.StatusTypes = unit.Orders.GetStatusTypes();
            return View(order);
        }

        // ========================================================================
        // [UPDATE] GET: Admin/Orders/ChangeStatus
        // ========================================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(int id, int OrderStatusId)
        {
            Order order = unit.Orders.Get(id);
            if (order == null)
            {
                TempData["msg"] = "Order not found";
                return RedirectToAction("Index");
            }

            order.OrderStatusId = OrderStatusId;
            order.Modified = DateTime.Now;

            unit.Orders.Update(order);
            unit.Complete();

            TempData["msg"] = "Order updated.";
            return RedirectToAction("Details", new { id });
        }

        // ========================================================================
        // [UPDATE] GET: Admin/Orders/Edit/{id}
        // ========================================================================
        public ActionResult Edit(int id)
        {
            Order order = unit.Orders.Get(id);
            if (order == null)
            {
                TempData["msg"] = "Order not found";
                return RedirectToAction("Index");
            }

            EditOrderViewModel orderVM = new EditOrderViewModel();

            if (TempData["Validation"] == null)
            {
                orderVM.Id = order.Id;
                orderVM.ShippingCity = order.ShippingCity;
                orderVM.ShippingAddress = order.ShippingAddress;
                orderVM.ShippingFirstName = order.ShippingFirstName;
                orderVM.ShippingLastName = order.ShippingLastName;
                orderVM.ShippingPhone = order.ShippingPhone;
                orderVM.ShippingPostCode = order.ShippingPostCode;
                orderVM.PaymentTypeId = order.PaymentTypeId;
                orderVM.OrderStatusId = order.OrderStatusId;
            }
            else
            {
                ViewData = (ViewDataDictionary)TempData["Validation"];
            }

            // get active payment types for PaymentTypeId dropdown
            ViewBag.Payments = unit.Orders.GetPaymentTypes(true).ToList();
            ViewBag.StatusTypes = unit.Orders.GetStatusTypes().ToList();
            ViewBag.OrderItems = order.Products.ToList();

            return View(orderVM);
        }

        // ========================================================================
        // [UPDATE] POST: Admin/Orders/Edit
        // ========================================================================
        [HttpPost]
        public ActionResult Edit(EditOrderViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Validation"] = ViewData;
                return RedirectToAction("Edit", new { vm.Id });
            }

            Order order = unit.Orders.Get(vm.Id);
            order.ShippingCity = vm.ShippingCity;
            order.ShippingAddress = vm.ShippingAddress;
            order.ShippingFirstName = vm.ShippingFirstName;
            order.ShippingLastName = vm.ShippingLastName;
            order.ShippingPhone = vm.ShippingPhone;
            order.ShippingPostCode = vm.ShippingPostCode;
            order.PaymentTypeId = vm.PaymentTypeId;
            order.OrderStatusId = vm.OrderStatusId;
            order.Modified = DateTime.Now;

            unit.Orders.Update(order);
            unit.Complete();

            TempData["msg"] = "Order updated.";
            return RedirectToAction("Details", new { vm.Id });
        }

        // ========================================================================
        // [DELETE] GET: Admin/Orders/Delete
        // ========================================================================
        public ActionResult Delete(int id)
        {
            Order order = unit.Orders.Get(id);

            if (order == null)
            {
                TempData["msg"] = $"Error. Order #{id} not found.";
            }
            else
            {
                unit.Orders.Remove(order);
                unit.Complete();
                TempData["msg"] = $"Order #{id} deleted.";
            }

            return RedirectToAction("Index");
        }

    }
}