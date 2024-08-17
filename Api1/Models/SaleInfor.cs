using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api1.Models
{
    [Table("SaleInfor")]

    public class SaleInfor:CommonAbstract
        
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int  Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Detail { get; set; }
    }
}
