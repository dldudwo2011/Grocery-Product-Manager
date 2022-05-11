using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DAL;
using Backend.Models.Queries;
using Backend.Entities;
using Product = Backend.Models.Queries.Product;
using Category = Backend.Models.Queries.Category;

namespace Backend.BLL
{
    public class GroceryListService
    {
        #region Constructor and DI fields
        private readonly GrocerylistContext _context;
        internal GroceryListService(GrocerylistContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries (reading the database)
        public List<Product> ListProducts(int pageNumber, int pageSize, out int totalRows)
        {
            totalRows = _context.Products.Count();
            var result = from product in _context.Products
                         select new Product
                         {
                             ProductId = product.ProductId,
                             Description = product.Description,
                             Price = product.Price,
                             SalePrice = product.Price - product.Discount,
                             Discount = product.Discount,
                             UnitSize = product.UnitSize,
                             Taxable = product.Taxable,
                             Photo = product.Photo
                         };
            int skipRows = (pageNumber - 1) * pageSize;
            return result.Skip(skipRows).Take(pageSize).ToList();
        }


        public List<Product> ListProducts(int categoryID, int pageNumber, int pageSize, out int totalRows)
        {
            var result = from product in _context.Products
                         where product.CategoryId == categoryID
                         select new Product
                         {
                             ProductId = product.ProductId,
                             Description = product.Description,
                             Price = product.Price,
                             SalePrice = product.Price - product.Discount,
                             Discount = product.Discount,
                             UnitSize = product.UnitSize,
                             Taxable = product.Taxable,
                             Photo = product.Photo
                         };

            totalRows = result.Count();

            int skipRows = (pageNumber - 1) * pageSize;
            return result.Skip(skipRows).Take(pageSize).ToList();

        }

        public List<Category> ListCategories()
        {
            var result = from category in _context.Categories
                         orderby category.Description
                         select new Category
                         {
                             CategoryID = category.CategoryId,
                             Description = category.Description
                         };
            return result.ToList();

        }

        #endregion

        #region CRUD behaviour
     
        public Models.Queries.Product LookupProduct(int productId)
        {
            // Remember to map our database Entity results from the lookup to our View Model type
            Models.Queries.Product result = null;
            var found = _context.Products.Find(productId);
            if (found != null)
                result = new()
                {
                    ProductId = found.ProductId,
                    Description = found.Description,
                    Price = found.Price,
                    Discount = found.Discount,
                    UnitSize = found.UnitSize,
                    CategoryId = found.CategoryId,
                    Taxable = found.Taxable,
                    Photo = found.Photo
                };
            return result;
        }

        public int AddProduct(Backend.Models.Queries.Product product)
        {
            // Validation of no duplicates
            if (_context.Products.Any(x => x.ProductId == product.ProductId))
                throw new ArgumentOutOfRangeException(nameof(product.ProductId), "A product already exists with that product id");

            Backend.Entities.Product newData = new()
            {
                // Transferring the data from our public model to the internal Entity type
                ProductId = product.ProductId,
                Description = product.Description,
                Price = product.Price,
                Discount = product.Discount,
                UnitSize = product.UnitSize,
                CategoryId = product.CategoryId,
                Taxable = product.Taxable,
                Photo = product.Photo
            };
            _context.Products.Add(newData);
            _context.SaveChanges(); // This is where the Transaction processing occurs.
            return newData.ProductId;
        }

        public void UpdateProduct(int productId, Backend.Models.Queries.Product product)
        {
            // Validation of no duplicates
            if (_context.Products.Any(x => x.ProductId == product.ProductId && x.ProductId != productId))
                throw new ArgumentOutOfRangeException(nameof(product.ProductId), $"Another product already exists with that product id; correct that product before reassigning the {product.ProductId} to this product");
            // Update
            var existing = _context.Products.Find(productId);
            if (existing is null)
                throw new ArgumentException("Could not find the specified product", nameof(productId));

            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Discount = product.Discount;
            existing.UnitSize = product.UnitSize;
            existing.CategoryId = product.CategoryId;
            existing.Taxable = product.Taxable;
            existing.Photo = product.Photo;
            _context.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            var existing = _context.Products.Find(productId);
            if (existing is null)
                throw new ArgumentException("Could not find the specified product", nameof(productId));
            _context.Products.Remove(existing);
            _context.SaveChanges();
        }
        #endregion
    }


}
