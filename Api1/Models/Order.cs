using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Order")]
    public class Order:CommonAbstract
    {
        public Order() {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OrderCode { get; set; }
        [Required(ErrorMessage = "Số điện thoại không để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int TypePayment { get; set; }
        public int Status { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public string? CustomerId { set;get; }
    }
}
