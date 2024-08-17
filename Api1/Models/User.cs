using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class User:CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Tên đăng nhập không được phép để trống!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được phép để trống!")]

        public string Password { get; set; }
    }
}
