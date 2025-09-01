using AspNetCoreMvcPractice.Core.DTOs;
using AspNetCoreMvcPractice.Core.Interfaces;
using AspNetCoreMvcPractice.Core.Services;
using AspNetCoreMvcPractice.Infrastructure.Models.Entities;
using Moq;
using Shouldly;
using System.Linq.Expressions;
using Xunit;

namespace AspNetCoreMvcPractice.Tests.Core.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IRepository<Product>> _mockProductRepo;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            // 1. 建立一個 IRepository<Product> 的模擬 (Mock) 物件
            _mockProductRepo = new Mock<IRepository<Product>>();

            // 2. 建立 ProductService 的實體，但注入的是模擬物件，而不是真實的資料庫倉儲
            _productService = new ProductService(_mockProductRepo.Object);
        }

        // 測試案例 1
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductDto_WhenProductExists()
        {
            // Arrange (安排): 設定測試情境
            var productId = 1;
            var product = new Product { ProductId = productId, ProductName = "Chai", Discontinued = false };

            // 設定模擬物件：當 GetByIdAsync(1) 被呼叫時，假裝從資料庫找到了 product 物件並回傳
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
                            .ReturnsAsync(product);

            // Act (執行): 呼叫要被測試的方法
            // 先接收完整的元組回傳值
            var resultTuple = await _productService.GetProductByIdAsync(productId);

            // 從元組中取出我們真正需要的 product 物件和 error message
            var resultDto = resultTuple.product;
            var errorMessage = resultTuple.errorMessage;

            // Assert (斷言): 驗證結果是否如預期
            // 1. 斷言錯誤訊息應該是 null，因為我們預期這次呼叫會成功
            errorMessage.ShouldBeNull();

            // 2. 斷言 DTO 物件不為 null，且內容正確
            resultDto.ShouldNotBeNull();
            resultDto.ProductId.ShouldBe(productId);
            resultDto.ProductName.ShouldBe("Chai");
        }

        // --- 測試案例 2 ---
        [Fact]
        public async Task CreateProductAsync_ShouldReturnError_WhenProductNameIsDuplicate()
        {
            // Arrange (安排)
            var productName = "Chai";
            var newProductDto = new ProductDto { ProductName = productName, UnitPrice = 18.00m };
            var existingProducts = new List<Product> { new Product { ProductId = 1, ProductName = productName } };

            // 設定模擬物件：當 ListAsync 被呼叫，且條件是尋找品名為 "Chai" 的產品時，
            // 假裝在資料庫中找到了 existingProducts 這個列表
            _mockProductRepo.Setup(repo => repo.ListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                            .ReturnsAsync(existingProducts);

            // Act (執行)
            var (success, errorMessage, newProduct) = await _productService.CreateProductAsync(newProductDto);

            // Assert (斷言)
            success.ShouldBeFalse(); // 驗證：執行結果應為失敗
            errorMessage.ShouldBe("產品名稱已經存在。"); // 驗證：錯誤訊息應符合預期
            newProduct.ShouldBeNull(); // 驗證：不應該回傳新的產品物件
        }
    }
}