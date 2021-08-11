using LinqKit;
using Shop.Models;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Shop.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        public ProductRepository(ApplicationDbContext dbContext, ICategoryRepository categoryRepository) : base(dbContext)
        {
            _categoryRepository = categoryRepository;
        }

        private List<int> selectIds = new List<int>();

        private void LoadCategoryIds(List<CategoryTree> items)
        {
            foreach (var item in items)
            {
                selectIds.Add(item.Category.Id);

                if (item.SubCategories.Count > 0)
                    LoadCategoryIds(item.SubCategories);
            }
        }


        public ProductPage GetProducts(
            int? categoryId = null,
            int? page = null,
            int? count = null,
            string orderBy = null,
            string search = null
            )
        {
            IQueryable<Product> products = Context.Products;
            if (categoryId != null)
            {
                selectIds = new List<int>();
                LoadCategoryIds(_categoryRepository.GetCategoryTree(categoryId));
                selectIds.Add((int)categoryId);
                products = products.Where(p => selectIds.Contains((int)p.CategoryId));
            }

            // search
            if (!String.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            // order
            products = products.OrderByDescending(p => p.Created); // default

            if (!String.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    // order by price
                    case "price":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    // order by name
                    case "name":
                        products = products.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        products = products.OrderByDescending(p => p.Name);
                        break;
                    // order by date
                    case "date":
                        products = products.OrderBy(p => p.Created);
                        break;
                    case "date_desc":
                        products = products.OrderByDescending(p => p.Created);
                        break;
                    default:
                        break;
                }
            }

            // pagination
            if (page == null || page <= 0)
                page = 1;
            if (count == null || count <= 0)
                count = 8; // default number of products per page

            int total = products.Count();

            int toSkip = (int)(count * (page - 1));
            products = products.Skip(toSkip).Take((int)count);

            ProductPage pp = new ProductPage
            {
                Products = products.ToList(),
                Total = total,
                CurrentPage = (int)page,
                PageSize = (int)count
            };

            double maxPages = (double)pp.Total / pp.PageSize;
            int lastPage = (int)Math.Ceiling(maxPages);
            pp.LastPage = lastPage == 0 ? 1 : lastPage;

            return pp;
        }

        // images
        public IEnumerable<ProductImage> GetProductImages(int productId)
        {
            return Context.ProductImages.Where(i => i.ProductId == productId);
        }

        public IEnumerable<Product> GetNewest(int n)
        {
            return Context.Products.OrderByDescending(p => p.Created).Take(n);
        }

        public void AddProductImage(ProductImage image)
        {
            Context.ProductImages.Add(image);
        }

        public void DeleteProductImage(ProductImage image)
        {
            Context.ProductImages.Remove(image);
        }

        public void DeleteProductImages(IEnumerable<ProductImage> images)
        {
            Context.ProductImages.RemoveRange(images);
        }

        // currency
        public Currency GetCurrency()
        {
            return Context.Currencies.First();
        }

        public void UpdateCurrency(Currency currency)
        {
            Context.Entry(currency).State = EntityState.Modified;
        }
    }
}