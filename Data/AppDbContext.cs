using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== QUAN HỆ 1-1: TaiKhoan ↔ KhachHang =====
            modelBuilder.Entity<KhachHang>()
                .HasOne(k => k.TaiKhoan)
                .WithOne(t => t.KhachHang)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan);

            // ===== UNIQUE INDEX (khớp với ràng buộc trong SQL) =====
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap).IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.Email).IsUnique();

            modelBuilder.Entity<LoaiSanPham>()
                .HasIndex(l => l.TenLoai).IsUnique();
        }
    }
}
