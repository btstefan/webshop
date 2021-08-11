using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Get Payment Types
        /// </summary>
        /// <param name="active">Empty for all, true for active</param>
        IEnumerable<PaymentType> GetPaymentTypes(bool active = false);

        /// <summary>
        /// Get total price of cart products
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns>Final price</returns>
        double GetTotal(List<CartItem> cartItems);

        /// <summary>
        /// Add range of products to order
        /// </summary>
        /// <param name="orderProducts"></param>
        void AddProducts(List<OrderProduct> orderProducts);

        /// <summary>
        /// Get all status types
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrderStatus> GetStatusTypes();
    }
}
