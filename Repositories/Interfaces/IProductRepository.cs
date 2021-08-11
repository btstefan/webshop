using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        ProductPage GetProducts(
            int? categoryId = null,
            int? page = null,
            int? count = null,
            string orderBy = null,
            string search = null
            );
        IEnumerable<ProductImage> GetProductImages(int productId);
        IEnumerable<Product> GetNewest(int n);
        void AddProductImage(ProductImage image);
        void DeleteProductImage(ProductImage image);
        void DeleteProductImages(IEnumerable<ProductImage> images);
        Currency GetCurrency();
        void UpdateCurrency(Currency currency);
    }
}
