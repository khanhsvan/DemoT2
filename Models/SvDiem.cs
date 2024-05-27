using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class SvDiem
    {
        public int MaDangKy { get; set; }
        public int MaGiaoVien { get; set; }
        public double DiemSo { get; set; }
        public DateTime NgayNhapDiem { get; set; }
        public string TenMonHoc { get; set; }
        public int SoTinChi { get; set; }
        public string TenGiaoVien { get; set; }
    }

}