using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class Diem
    {
        [Key]
        public int MaDiem { get; set; }

        public int MaDangKy { get; set; }
        public DangKy DangKy { get; set; }

        public int MaGiaoVien { get; set; }
        public GiaoVien GiaoVien { get; set; }

        public decimal DiemSo { get; set; }

        public DateTime NgayNhapDiem { get; set; }
    }
}