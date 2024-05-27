using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class GiaoVien
    {
        [Key]
        public int MaGiaoVien { get; set; }

        [Required]
        [StringLength(50)]
        public string Ho { get; set; }

        [Required]
        [StringLength(50)]
        public string Ten { get; set; }

        [StringLength(50)]
        public string Khoa { get; set; }
    }

}