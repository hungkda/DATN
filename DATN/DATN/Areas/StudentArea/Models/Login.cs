using System.ComponentModel.DataAnnotations;

namespace DATN.Areas.StudentArea.Models
{
    public class Login
    {
        [Required(ErrorMessage = "UserName không được để trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
