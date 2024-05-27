using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class TaiKhoan
    {
        [Key]
        public int TaiKhoanId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        public int? SinhVienId { get; set; }

        public int? GiaoVienId { get; set; }

        // Navigation properties
        [ForeignKey("SinhVienId")]
        public virtual SinhVien SinhVien { get; set; }

        [ForeignKey("GiaoVienId")]
        public virtual GiaoVien GiaoVien { get; set; }
    }
}