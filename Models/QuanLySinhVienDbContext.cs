using System.Data.Entity;
using DemoT2.Models; // Add this line to include the model classes

namespace DemoT2.Models
{
    public class QuanLySinhVienDbContext : DbContext
    {
        public QuanLySinhVienDbContext() : base("name=DangNhap")
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<GiaoVien> GiaoViens { get; set; }
        public DbSet<MonHoc> MonHocs { get; set; } // Add DbSet for MonHoc
        public DbSet<DangKy> DangKies { get; set; } // Add DbSet for DangKy
        public DbSet<Diem> Diems { get; set; } // Add DbSet for Diem

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional model configuration

            modelBuilder.Entity<TaiKhoan>()
                .HasOptional(t => t.SinhVien)
                .WithMany()
                .HasForeignKey(t => t.SinhVienId);

            modelBuilder.Entity<TaiKhoan>()
                .HasOptional(t => t.GiaoVien)
                .WithMany()
                .HasForeignKey(t => t.GiaoVienId);

            // If needed, configure relationships for other tables here
            // For example:

            modelBuilder.Entity<DangKy>()
                .HasRequired(d => d.SinhVien)
                .WithMany()
                .HasForeignKey(d => d.MaSinhVien);

            modelBuilder.Entity<DangKy>()
                .HasRequired(d => d.MonHoc)
                .WithMany()
                .HasForeignKey(d => d.MaMonHoc);

            modelBuilder.Entity<Diem>()
                .HasRequired(d => d.DangKy)
                .WithMany()
                .HasForeignKey(d => d.MaDangKy);

            modelBuilder.Entity<Diem>()
                .HasRequired(d => d.GiaoVien)
                .WithMany()
                .HasForeignKey(d => d.MaGiaoVien);
        }
    }
}
