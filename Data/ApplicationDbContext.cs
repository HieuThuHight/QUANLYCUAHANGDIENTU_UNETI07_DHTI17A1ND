using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; } = null!;
        public DbSet<SanPham> SanPhams { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<DonHang> DonHangs { get; set; } = null!;
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChiTietDonHang>()
                .HasKey(ct => new { ct.MaDonHang, ct.MaSanPham });
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();
            modelBuilder.Entity<LoaiSanPham>()
                .HasIndex(l => l.TenLoai)
                .IsUnique();
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.KhachHang)
                .WithOne(k => k.TaiKhoan)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LoaiSanPham>()
                .HasMany(l => l.SanPhams)
                .WithOne(s => s.LoaiSanPham)
                .HasForeignKey(s => s.MaLoai)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KhachHang>()
                .HasMany(k => k.DonHangs)
                .WithOne(d => d.KhachHang)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DonHang>()
                .HasMany(d => d.ChiTietDonHangs)
                .WithOne(c => c.DonHang)
                .HasForeignKey(c => c.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SanPham>()
                .HasMany(s => s.ChiTietDonHangs)
                .WithOne(c => c.SanPham)
                .HasForeignKey(c => c.MaSanPham)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LoaiSanPham>().HasData(
                new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại", MoTa = "Điện thoại di động", TrangThai = true },
                new LoaiSanPham { MaLoai = 2, TenLoai = "Laptop", MoTa = "Máy tính xách tay", TrangThai = true }
            );
            modelBuilder.Entity<SanPham>().HasData(
                new SanPham
                {
                    MaSanPham = 1,
                    TenSanPham = "iPhone 15",
                    MaLoai = 1,
                    ThuongHieu = "Apple",
                    XuatXu = "Mỹ",
                    DonGia = 20000000m,
                    SoLuongTon = 50,
                    ThoiGianBaoHanh = 12,
                    MoTa = "Điện thoại Apple iPhone 15",
                    TrangThai = "Đang kinh doanh"
                },
                new SanPham
                {
                    MaSanPham = 2,
                    TenSanPham = "Laptop Dell XPS 13",
                    MaLoai = 2,
                    ThuongHieu = "Dell",
                    XuatXu = "Mỹ",
                    DonGia = 30000000m,
                    SoLuongTon = 30,
                    ThoiGianBaoHanh = 24,
                    MoTa = "Laptop Dell XPS 13",
                    TrangThai = "Đang kinh doanh"
                }
            );
            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan { MaTaiKhoan = 1, TenDangNhap = "admin", MatKhau = "123456", HoTen = "Quản trị viên", Email = "admin@example.com", VaiTro = "Admin", TrangThai = true },
                new TaiKhoan { MaTaiKhoan = 2, TenDangNhap = "khachhang1", MatKhau = "123456", HoTen = "Nguyễn Văn A", Email = "khachhang1@example.com", VaiTro = "KhachHang", TrangThai = true }
            );
            modelBuilder.Entity<KhachHang>().HasData(
                new KhachHang
                {
                    MaKhachHang = 1,
                    MaTaiKhoan = 2,
                    HoTen = "Nguyễn Văn A",
                    NgaySinh = new DateTime(2000, 1, 1),
                    GioiTinh = "Nam",
                    SoDienThoai = "0900000000",
                    Email = "khachhang1@example.com",
                    DiaChi = "Hà Nội",
                    NgayDangKy = new DateTime(2026, 1, 1),
                    TrangThai = true
                }
            );
        }
    }
}
