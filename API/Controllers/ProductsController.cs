using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class ProductsController(IProductRepository productRepository) : ControllerBase
        {
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, 
            string? type, string? sort)
            {
                return Ok(await productRepository.GetProductsAsync(brand, type, sort));
            }


            [HttpGet("{id}")]
            public async Task<ActionResult<Product>> GetProduct(int id)
            {
                var product = await productRepository.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                return product;
            }                                               

            [HttpPost]
            public async Task<ActionResult<Product>> CreateProduct(Product product)
            {
                productRepository.AddProduct(product);
                await productRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProduct(int id, Product product)
            {
                if (id != product.Id || !ProductExists(id)) return BadRequest("Cannot update this product");

                await productRepository.UpdateProductAsync(product);

                if (await productRepository.SaveChangesAsync())
                {
                    return NoContent();
                };
                 return BadRequest("Problem updating the product");
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProduct(int id)
            {
                var product = await productRepository.GetProductByIdAsync(id);

                if (product == null) return NotFound();

                productRepository.DeleteProduct(product);

                await productRepository.SaveChangesAsync();

                return NoContent();
            }
    
            [HttpGet("brands")]
            public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
            {
                return Ok(await productRepository.GetBrandsAsync());
            }
            
            [HttpGet("types")]
            public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
            {
                return Ok(await productRepository.GetTypesAsync());
            }
            private bool ProductExists(int id)
            {
                return productRepository.ProductExistsAsync(id).Result;
            }
        }
}