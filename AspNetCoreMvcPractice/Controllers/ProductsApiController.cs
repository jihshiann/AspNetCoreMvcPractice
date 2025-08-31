// Controllers/ProductsApiController.cs
using AspNetCoreMvcPractice.Core.DTOs;
using AspNetCoreMvcPractice.Core.Interfaces; // 引用 Core 的介面
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMvcPractice.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;

        // *** 現在注入的是 IProductService ***
        public ProductsApiController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            return Ok(await _productService.GetProductsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var (product, errorMessage) = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = errorMessage });
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, errorMessage, newProduct) = await _productService.CreateProductAsync(productDto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return BadRequest(ModelState);
            }
            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.ProductId }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            if (id != productDto.ProductId) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, errorMessage) = await _productService.UpdateProductAsync(id, productDto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return BadRequest(ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var (success, errorMessage) = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound(new { message = errorMessage });
            return NoContent();
        }
    }
}