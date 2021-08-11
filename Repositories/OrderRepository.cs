using Shop.Models;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<PaymentType> GetPaymentTypes(bool active = false)
        {
            if (active)
                return Context.PaymentTypes.Where(t => t.IsActive == true);
            return Context.PaymentTypes;
        }

        public IEnumerable<OrderStatus> GetStatusTypes()
        {
            return Context.OrderStatus;
        }

        public double GetTotal(List<CartItem> cartItems)
        {
            double total = 0;
            if (cartItems != null)
            {
                foreach (var item in cartItems)
                    total += item.Product.Price * item.Quantity;
            }
            return total;
        }

        public void AddProducts(List<OrderProduct> orderProducts)
        {
            Context.OrderProducts.AddRange(orderProducts);
        }
    }
}