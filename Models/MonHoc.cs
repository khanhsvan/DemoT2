using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class MonHoc
    {
        [Key]
        public int MaMonHoc { get; set; }

        [Required]
        [StringLength(100)]
        public string TenMonHoc { get; set; }

        public int SoTinChi { get; set; }

        [StringLength(50)]
        public string Khoa { get; set; }
    }

}