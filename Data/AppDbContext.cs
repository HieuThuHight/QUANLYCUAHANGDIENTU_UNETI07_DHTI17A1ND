// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: DbContext + cau hinh quan he

using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Models.Entities;
using System.Reflection.Emit;

namespace QuanLyCuaHangDienTu.Data
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
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<YeuCauHoanHang> YeuCauHoanHangs { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<TaiKhoan>().HasIndex(t => t.TenDangNhap).IsUnique();
            modelBuilder.Entity<LoaiSanPham>().HasIndex(l => l.TenLoai).IsUnique();
            modelBuilder.Entity<Voucher>().HasIndex(v => v.MaCode).IsUnique();

            // 1-1 TaiKhoan - KhachHang
            modelBuilder.Entity<KhachHang>()
                .HasOne(k => k.TaiKhoan).WithOne(t => t.KhachHang)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n LoaiSanPham - SanPham
            modelBuilder.Entity<SanPham>()
                .HasOne(s => s.LoaiSanPham).WithMany(l => l.SanPhams)
                .HasForeignKey(s => s.MaLoai)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n KhachHang - DonHang
            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.KhachHang).WithMany(k => k.DonHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n DonHang - ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(c => c.DonHang).WithMany(d => d.ChiTietDonHangs)
                .HasForeignKey(c => c.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-n SanPham - ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(c => c.SanPham).WithMany(s => s.ChiTietDonHangs)
                .HasForeignKey(c => c.MaSanPham)
                .OnDelete(DeleteBehavior.Restrict);

            // Voucher - DonHang
            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.Voucher).WithMany(v => v.DonHangs)
                .HasForeignKey(d => d.MaVoucher)
                .OnDelete(DeleteBehavior.SetNull);

            // DonHang - YeuCauHoanHang
            modelBuilder.Entity<YeuCauHoanHang>()
                .HasOne(y => y.DonHang).WithMany(d => d.YeuCauHoanHangs)
                .HasForeignKey(y => y.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(p => p.TaiKhoan).WithMany()
                .HasForeignKey(p => p.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}