using AspNetCoreMvcPractice.Core.DTOs;
using AspNetCoreMvcPractice.Core.Interfaces;
using AspNetCoreMvcPractice.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMvcPractice.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;

        public ProductService(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepo.ListAsync(p => !p.Discontinued);
            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock
            });
        }

        public async Task<(ProductDto? product, string? errorMessage)> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null || product.Discontinued)
            {
                return (null, "產品不存在或已停用。");
            }

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock
            };
            return (productDto, null);
        }

        public async Task<(bool Success, string? ErrorMessage, ProductDto? NewProduct)> CreateProductAsync(ProductDto productDto)
        {
            var existingProducts = await _productRepo.ListAsync(p => p.ProductName == productDto.ProductName);
            if (existingProducts.Any())
            {
                return (false, "產品名稱已經存在。", null);
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                UnitPrice = productDto.UnitPrice,
                UnitsInStock = productDto.UnitsInStock ?? 0,
                Discontinued = false
            };

            await _productRepo.AddAsync(product);
            productDto.ProductId = product.ProductId; // 回寫由資料庫產生的新ID

            return (true, null, productDto);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(int id, ProductDto productDto)
        {
            var existingProducts = await _productRepo.ListAsync(p => p.ProductName == productDto.ProductName && p.ProductId != id);
            if (existingProducts.Any())
            {
                return (false, "此產品名稱已被其他產品使用。");
            }

            var productToUpdate = await _productRepo.GetByIdAsync(id);
            if (productToUpdate == null || productToUpdate.Discontinued)
            {
                return (false, "找不到要更新的產品。");
            }

            productToUpdate.ProductName = productDto.ProductName;
            productToUpdate.UnitPrice = productDto.UnitPrice;
            productToUpdate.UnitsInStock = productDto.UnitsInStock ?? 0;

            await _productRepo.UpdateAsync(productToUpdate);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteProductAsync(int id)
        {
            var productToDelete = await _productRepo.GetByIdAsync(id);
            if (productToDelete == null)
            {
                return (false, "找不到要刪除的產品。");
            }

            productToDelete.Discontinued = true; // 軟刪除
            await _productRepo.UpdateAsync(productToDelete);

            return (true, null);
        }
    }
}