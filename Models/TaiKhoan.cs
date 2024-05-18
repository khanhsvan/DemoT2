using System;
using System.ComponentModel.DataAnnotations;

namespace DemoT2.Models
{
    public class TaiKhoan
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string taikhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string matkhau { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string hoten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string sodienthoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ngaysinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; }

        public string role { get; set; }
    }
}
