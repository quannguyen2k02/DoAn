using API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api1.Data
{
    public class ProductDTO
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public string Detail { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceSale { get; set; }
        public int Quantity { get; set; }
        public bool isHot { set; get; }
        public int ProductCategoryId { get; set; }

        public IFormFile? Image { get; set; } // Đối tượng đại diện cho tệp ảnh

    }
}
