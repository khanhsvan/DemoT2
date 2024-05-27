using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class svDkModel
    {
        [Key]
        public int MaDangKy { get; set; }

        public int MaSinhVien { get; set; }
        public SinhVien SinhVien { get; set; }

        public int MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }

        public MonHoc MonHoc { get; set; }

        public DateTime NgayDangKy { get; set; }
    }
}