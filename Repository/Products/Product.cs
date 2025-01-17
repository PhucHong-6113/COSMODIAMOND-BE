﻿using DAO;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Repository.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly DIAMOND_DBContext _context;

        public ProductRepository(DIAMOND_DBContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Diamond)
                .Include(p => p.Cover).ThenInclude(p => p.CoverMetaltypes)
                .Include(p => p.Metaltype)
                .Include(p => p.Size)
                .ToList();
        }
        public Product GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Diamond)
                .Include(p => p.Cover).ThenInclude(p=>p.CoverMetaltypes)
                .Include(p => p.Metaltype)
                .Include(p => p.Size)
                .FirstOrDefault(p => p.ProductId == productId);
        }
        public Category GetCategoryById(int categoryId)
        {
            return _context.Categories.FirstOrDefault( c=> c.CategoryId==categoryId);
        }

        public Product AddProduct(Product product)
        {
            _context.Products.Add(product); 
            _context.SaveChanges();
            return product;
        }

        public Product UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return product;
        }

        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public List<ProductQuantity> GetMostOrderedProductsBySubCategory(int count, int subcate)
        {
            var result = _context.ProductOrders.Include(c => c.Product).ThenInclude(c => c.Cover).ThenInclude(c => c.Category)
                .Where(po => po.Product.Cover.CategoryId == subcate)
                .GroupBy(po => po.ProductId)
                .Select(g => new ProductQuantity
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(po => po.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(count)
                .ToList();
            return result;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
    public class ProductQuantity
    {
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
    }
}
