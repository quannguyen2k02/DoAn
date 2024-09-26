using System.ComponentModel.DataAnnotations;

namespace Api1.Data
{
    public class ProductCategoryDTO
    {
        public int?  Id { get; set; }
        public string Title { get; set; }
        [StringLength(150)]
        public string? Description { get; set; }
        public IFormFile? Image { get; set; } // Đối tượng đại diện cho tệp ảnh
    }
}
