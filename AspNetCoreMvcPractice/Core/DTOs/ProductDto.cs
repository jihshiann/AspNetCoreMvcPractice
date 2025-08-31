using System.ComponentModel.DataAnnotations;
namespace AspNetCoreMvcPractice.Core.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "產品名稱為必填欄位。")]
        public string? ProductName { get; set; }
        [Required(ErrorMessage = "產品單價為必填欄位。")]
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
    }
}