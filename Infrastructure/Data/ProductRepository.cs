using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;
        public ProductRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, 
        string? type, string? sort)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.Type == type);
            }
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "priceasc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "pricedesc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    default:
                        query = query.OrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            return await query.ToListAsync();
        }
        
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.PictureUrl = product.PictureUrl;
                existingProduct.Brand = product.Brand;
                existingProduct.Type = product.Type;

                _context.Products.Update(existingProduct);
            }
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        public Task<IReadOnlyList<string>> GetBrandsAsync()
        {
            var brands = _context.Products.Select(p => p.Brand).Distinct().ToList();
            return Task.FromResult((IReadOnlyList<string>)brands);
        }

        public Task<IReadOnlyList<string>> GetTypesAsync()
        {
            var types = _context.Products.Select(p => p.Type).Distinct().ToList();
            return Task.FromResult((IReadOnlyList<string>)types);
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
  
    }
}