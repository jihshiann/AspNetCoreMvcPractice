using AspNetCoreMvcPractice.Core.DTOs;

namespace AspNetCoreMvcPractice.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<(ProductDto? product, string? errorMessage)> GetProductByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, ProductDto? NewProduct)> CreateProductAsync(ProductDto productDto);
        Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(int id, ProductDto productDto);
        Task<(bool Success, string? ErrorMessage)> DeleteProductAsync(int id);
    }
}