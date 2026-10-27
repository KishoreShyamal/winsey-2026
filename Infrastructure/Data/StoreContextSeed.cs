using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context)
        {
            if (!await context.Products.AnyAsync())
            {
                var productsPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "products.json");
                var productsData = await File.ReadAllTextAsync(productsPath);
                // Parse the JSON data and add products to the context
                var products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(productsData);
                if (products == null)
                {
                    throw new InvalidOperationException("Failed to deserialize product data");
                }
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}