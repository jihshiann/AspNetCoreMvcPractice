using AspNetCoreMvcPractice.Data;
using AspNetCoreMvcPractice.DTOs;
using AspNetCoreMvcPractice.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMvcPractice.Controllers
{
    [Route("api/products")] 
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public ProductsApiController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                                         .Where(p => !p.Discontinued)
                                         .Select(p => new ProductDto
                                         {
                                             ProductId = p.ProductId,
                                             ProductName = p.ProductName,
                                             UnitPrice = p.UnitPrice,
                                             UnitsInStock = p.UnitsInStock
                                         })
                                         .ToListAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.Discontinued)
            {
                return NotFound();
            }

            // Product 轉換為 ProductDto
            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock
            };

            return Ok(productDto);
        }

        // POST: api/products (add)
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                UnitPrice = productDto.UnitPrice,
                UnitsInStock = productDto.UnitsInStock,
                Discontinued = false
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.ProductId = product.ProductId;
            // 201 Created 附上新product
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, productDto);
        }

        // PUT: api/products/5 (update)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest();
            }

            var productToUpdate = await _context.Products.FindAsync(id);
            if (productToUpdate == null || productToUpdate.Discontinued)
            {
                return NotFound();
            }

            productToUpdate.ProductName = productDto.ProductName;
            productToUpdate.UnitPrice = productDto.UnitPrice;
            productToUpdate.UnitsInStock = productDto.UnitsInStock;

            await _context.SaveChangesAsync();

            var updatedProductDto = new ProductDto
            {
                ProductId = productToUpdate.ProductId,
                ProductName = productToUpdate.ProductName,
                UnitPrice = productToUpdate.UnitPrice,
                UnitsInStock = productToUpdate.UnitsInStock
            };

            //  200 OK 附更新後product
            return Ok(updatedProductDto);
        }

        // DELETE: api/products/5 (delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Discontinued = true; // 將產品標記為已停用
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}