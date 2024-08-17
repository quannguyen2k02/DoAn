using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    [Table("ProductCategory")]
    public class ProductCategory:CommonAbstract
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Tiêu đề không được để trống!")]
        [StringLength(150)]
        public string Title { get; set; }
        [StringLength(150)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
