using Microsoft.AspNet.Identity;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork unit;

        public ProductsController(IUnitOfWork unit)
        {
            this.unit = unit;
        }

        // ========================================================================
        // [READ] GET: Admin/Product
        // ========================================================================
        public ActionResult Index()
        {
            List<Product> ProductList = unit.Products.GetAll().ToList();

            return View(ProductList);
        }

        // PRIVATE: Save product small image
        private void SaveThumbnail(string path, HttpPostedFileBase image)
        {
            // Generate thumbnail, max size = 400x400
            float width = 400;
            float height = 400;

            var img = Image.FromStream(image.InputStream);
            float scale = Math.Min(width / img.Width, height / img.Height);
            int scaleWidth = (int)(img.Width * scale);
            int scaleHeight = (int)(img.Height * scale);

            using (Image thumbnail = img.GetThumbnailImage(scaleWidth, scaleHeight, new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
            {
                thumbnail.Save(path, ImageFormat.Png);
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        // ========================================================================
        // [INSERT] GET: Admin/Products/Add
        // ========================================================================
        public ActionResult Add()
        {
            if (TempData["ViewData"] != null)
                ViewData = (ViewDataDictionary)TempData["ViewData"];

            ViewBag.CategoryTree = unit.Categories.GetCategoryTree();
            ViewBag.Currency = unit.Products.GetCurrency();

            return View();
        }

        // ========================================================================
        // [INSERT] POST: Admin/Products/Add
        // ========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddProductViewModel p)
        {
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Add");
            }

            // Thumbnail upload
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss_") + Guid.NewGuid().ToString().Substring(0, 7) + ".png";
            string tPath = Path.Combine(Server.MapPath("~/Upload/img/products/thumbnails"), fileName); // thumbnail

            SaveThumbnail(tPath, p.Thumbnail);

            var userId = Guid.Parse(User.Identity.GetUserId());

            // Add product to db
            Product newProduct = new Product()
            {
                Name = p.Name,
                UserId = userId,
                CategoryId = p.CategoryId,
                Price = p.Price,
                CurrencyId = unit.Products.GetCurrency().Id,
                Thumbnail = fileName,
                Description = p.Description,
                Created = DateTime.Now
            };

            unit.Products.Add(newProduct);
            unit.Complete();

            // Images upload 
            int productId = newProduct.Id;

            if (p.Images != null && p.Images.Count > 1)
            {
                foreach (var formImage in p.Images)
                {

                    if (formImage != null)
                    {
                        // generate path
                        string imgName = DateTime.Now.ToString("yyyyMMdd-HHmmss_") + Guid.NewGuid().ToString().Substring(0, 7) + ".png";
                        string path = Path.Combine(Server.MapPath("~/Upload/img/products"), imgName); // product image

                        // save image
                        Image productImage = Image.FromStream(formImage.InputStream);
                        productImage.Save(path, ImageFormat.Png);

                        // add to db
                        ProductImage newImage = new ProductImage()
                        {
                            ProductId = productId,
                            Source = imgName
                        };

                        unit.Products.AddProductImage(newImage);
                    }
                }
                unit.Complete();
            }
            TempData["msg"] = "Product added successfuly.";
            return RedirectToAction("Index");
        }

        // ========================================================================
        // [UPDATE] GET: Admin/Product/Edit/id
        // ========================================================================
        public ActionResult Edit(int id)
        {
            Product product = unit.Products.Get(id);

            if (product == null)
            {
                TempData["msg"] = "Product not found";
                return RedirectToAction("Products");
            }

            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = product
            };
            ViewBag.CategoryTree = unit.Categories.GetCategoryTree();

            return View(productViewModel);
        }

        // ========================================================================
        // [UPDATE] POST: Admin/Product/Edit
        // ========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryTree = unit.Categories.GetCategoryTree();
                return View(vm);
            }

            if (vm.ActiveImages != null && vm.ActiveImages.Count() > 0)
            {
                // image ids for delete
                IEnumerable<int> deleteIds = vm.ActiveImages;
                // all product images
                IEnumerable<ProductImage> productImages = unit.Products.GetProductImages(vm.Product.Id);

                if (productImages.Count() > 0)
                {
                    IEnumerable<ProductImage> toDelete = productImages.Join(deleteIds, image => image.Id, delete => delete, (image, delete) => image);

                    if (toDelete.Any())
                    {
                        // delete from db
                        unit.Products.DeleteProductImages(toDelete);

                        // delete from storage
                        foreach (var image in toDelete.ToList())
                        {
                            string path = Path.Combine(Server.MapPath("~/Upload/img/products"), image.Source);
                            System.IO.File.Delete(path);
                        }
                    }
                }
            }

            // update product
            Product product = vm.Product;

            if (vm.Thumbnail != null)
            {
                // save new
                var name = DateTime.Now.ToString("yyyyMMdd-HHmmss_") + Guid.NewGuid().ToString().Substring(0, 7) + ".png";
                string path = Path.Combine(Server.MapPath("~/Upload/img/products/thumbnails"), name);
                SaveThumbnail(path, vm.Thumbnail);

                // delete old
                string delPath = Path.Combine(Server.MapPath("~/Upload/img/products/thumbnails"), product.Thumbnail);
                System.IO.File.Delete(delPath);

                // register new
                product.Thumbnail = name;
            }

            // Add product images
            if (vm.Images != null && vm.Images.Count > 1)
            {
                foreach (var formImage in vm.Images)
                {

                    if (formImage != null)
                    {
                        // generate path
                        string imgName = DateTime.Now.ToString("yyyyMMdd-HHmmss_") + Guid.NewGuid().ToString().Substring(0, 7) + ".png";
                        string path = Path.Combine(Server.MapPath("~/Upload/img/products"), imgName); // product image

                        // save image
                        Image productImage = Image.FromStream(formImage.InputStream);
                        productImage.Save(path, ImageFormat.Png);

                        // add to db
                        ProductImage newImage = new ProductImage()
                        {
                            ProductId = product.Id,
                            Source = imgName
                        };

                        unit.Products.AddProductImage(newImage);
                    }
                }
            }

            product.Modified = DateTime.Now;

            // update product
            unit.Products.Update(product);
            unit.Complete();

            TempData["msg"] = "Product updated successfully";
            return RedirectToAction("Index");
        }

        // ========================================================================
        // [DELETE] GET: Admin/Product/Delete/id
        // ========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Product product = unit.Products.Get(id);
            string thumbnail = product.Thumbnail;
            List<ProductImage> images = product.ProductImages.ToList();

            if (product == null)
            {
                TempData["msg"] = "Product not found.";
                return RedirectToAction("Index");
            }

            unit.Products.Remove(product);
            unit.Complete();

            // delete thumbnail
            string thumnailPath = Path.Combine(Server.MapPath("~/Upload/img/products/thumbnails"), thumbnail);
            System.IO.File.Delete(thumnailPath);

            // delete images
            if (images != null)
            {
                foreach (ProductImage image in images)
                {
                    string imagePath = Path.Combine(Server.MapPath("~/Upload/img/products"), image.Source);
                    System.IO.File.Delete(imagePath);
                }
            }

            TempData["msg"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}