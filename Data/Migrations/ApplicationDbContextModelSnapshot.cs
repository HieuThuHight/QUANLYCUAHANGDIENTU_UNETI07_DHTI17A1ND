using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
#nullable disable
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);
            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.ChiTietDonHang", b =>
                {
                    b.Property<int>("MaDonHang")
                        .HasColumnType("int");
                    b.Property<int>("MaSanPham")
                        .HasColumnType("int");
                    b.Property<decimal>("DonGia")
                        .HasColumnType("decimal(18,2)");
                    b.Property<int>("SoLuong")
                        .HasColumnType("int");
                    b.Property<decimal>("ThanhTien")
                        .HasColumnType("decimal(18,2)");
                    b.HasKey("MaDonHang", "MaSanPham");
                    b.HasIndex("MaSanPham");
                    b.ToTable("ChiTietDonHangs");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.DonHang", b =>
                {
                    b.Property<int>("MaDonHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaDonHang"));
                    b.Property<string>("DiaChiGiaoHang")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<int>("MaKhachHang")
                        .HasColumnType("int");
                    b.Property<DateTime>("NgayDat")
                        .HasColumnType("datetime2");
                    b.Property<decimal>("TongTien")
                        .HasColumnType("decimal(18,2)");
                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");
                    b.HasKey("MaDonHang");
                    b.HasIndex("MaKhachHang");
                    b.ToTable("DonHangs");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.KhachHang", b =>
                {
                    b.Property<int>("MaKhachHang")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaKhachHang"));
                    b.Property<string>("DiaChi")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<string>("GioiTinh")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");
                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<int>("MaTaiKhoan")
                        .HasColumnType("int");
                    b.Property<DateTime>("NgayDangKy")
                        .HasColumnType("datetime2");
                    b.Property<DateTime?>("NgaySinh")
                        .HasColumnType("datetime2");
                    b.Property<string>("SoDienThoai")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");
                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");
                    b.HasKey("MaKhachHang");
                    b.HasIndex("MaTaiKhoan")
                        .IsUnique();
                    b.ToTable("KhachHangs");
                    b.HasData(
                        new
                        {
                            MaKhachHang = 1,
                            DiaChi = "Hà Nội",
                            Email = "khachhang1@example.com",
                            GioiTinh = "Nam",
                            HoTen = "Nguyễn Văn A",
                            MaTaiKhoan = 2,
                            NgayDangKy = new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            NgaySinh = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            SoDienThoai = "0900000000",
                            TrangThai = true
                        });
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.LoaiSanPham", b =>
                {
                    b.Property<int>("MaLoai")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaLoai"));
                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("TenLoai")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");
                    b.HasKey("MaLoai");
                    b.HasIndex("TenLoai")
                        .IsUnique();
                    b.ToTable("LoaiSanPhams");
                    b.HasData(
                        new
                        {
                            MaLoai = 1,
                            MoTa = "Điện thoại di động",
                            TenLoai = "Điện thoại",
                            TrangThai = true
                        },
                        new
                        {
                            MaLoai = 2,
                            MoTa = "Máy tính xách tay",
                            TenLoai = "Laptop",
                            TrangThai = true
                        });
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.SanPham", b =>
                {
                    b.Property<int>("MaSanPham")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaSanPham"));
                    b.Property<decimal>("DonGia")
                        .HasColumnType("decimal(18,2)");
                    b.Property<int>("MaLoai")
                        .HasColumnType("int");
                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");
                    b.Property<int>("SoLuongTon")
                        .HasColumnType("int");
                    b.Property<string>("TenSanPham")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");
                    b.Property<int?>("ThoiGianBaoHanh")
                        .HasColumnType("int");
                    b.Property<string>("ThuongHieu")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");
                    b.Property<string>("XuatXu")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.HasKey("MaSanPham");
                    b.HasIndex("MaLoai");
                    b.ToTable("SanPhams");
                    b.HasData(
                        new
                        {
                            MaSanPham = 1,
                            DonGia = 20000000m,
                            MaLoai = 1,
                            MoTa = "Điện thoại Apple iPhone 15",
                            SoLuongTon = 50,
                            TenSanPham = "iPhone 15",
                            ThoiGianBaoHanh = 12,
                            ThuongHieu = "Apple",
                            TrangThai = "Đang kinh doanh",
                            XuatXu = "Mỹ"
                        },
                        new
                        {
                            MaSanPham = 2,
                            DonGia = 30000000m,
                            MaLoai = 2,
                            MoTa = "Laptop Dell XPS 13",
                            SoLuongTon = 30,
                            TenSanPham = "Laptop Dell XPS 13",
                            ThoiGianBaoHanh = 24,
                            ThuongHieu = "Dell",
                            TrangThai = "Đang kinh doanh",
                            XuatXu = "Mỹ"
                        });
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.TaiKhoan", b =>
                {
                    b.Property<int>("MaTaiKhoan")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaTaiKhoan"));
                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<string>("MatKhau")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<string>("TenDangNhap")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");
                    b.Property<bool>("TrangThai")
                        .HasColumnType("bit");
                    b.Property<string>("VaiTro")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");
                    b.HasKey("MaTaiKhoan");
                    b.HasIndex("TenDangNhap")
                        .IsUnique();
                    b.ToTable("TaiKhoans");
                    b.HasData(
                        new
                        {
                            MaTaiKhoan = 1,
                            Email = "admin@example.com",
                            HoTen = "Quản trị viên",
                            MatKhau = "123456",
                            TenDangNhap = "admin",
                            TrangThai = true,
                            VaiTro = "Admin"
                        },
                        new
                        {
                            MaTaiKhoan = 2,
                            Email = "khachhang1@example.com",
                            HoTen = "Nguyễn Văn A",
                            MatKhau = "123456",
                            TenDangNhap = "khachhang1",
                            TrangThai = true,
                            VaiTro = "KhachHang"
                        });
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.ChiTietDonHang", b =>
                {
                    b.HasOne("quanlycuahangdientu_uneti07_dhti17a1nd.Models.DonHang", "DonHang")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("MaDonHang")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("quanlycuahangdientu_uneti07_dhti17a1nd.Models.SanPham", "SanPham")
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("MaSanPham")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("DonHang");
                    b.Navigation("SanPham");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.DonHang", b =>
                {
                    b.HasOne("quanlycuahangdientu_uneti07_dhti17a1nd.Models.KhachHang", "KhachHang")
                        .WithMany("DonHangs")
                        .HasForeignKey("MaKhachHang")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("KhachHang");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.KhachHang", b =>
                {
                    b.HasOne("quanlycuahangdientu_uneti07_dhti17a1nd.Models.TaiKhoan", "TaiKhoan")
                        .WithOne("KhachHang")
                        .HasForeignKey("quanlycuahangdientu_uneti07_dhti17a1nd.Models.KhachHang", "MaTaiKhoan")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("TaiKhoan");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.SanPham", b =>
                {
                    b.HasOne("quanlycuahangdientu_uneti07_dhti17a1nd.Models.LoaiSanPham", "LoaiSanPham")
                        .WithMany("SanPhams")
                        .HasForeignKey("MaLoai")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("LoaiSanPham");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.DonHang", b =>
                {
                    b.Navigation("ChiTietDonHangs");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.KhachHang", b =>
                {
                    b.Navigation("DonHangs");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.LoaiSanPham", b =>
                {
                    b.Navigation("SanPhams");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.SanPham", b =>
                {
                    b.Navigation("ChiTietDonHangs");
                });
            modelBuilder.Entity("quanlycuahangdientu_uneti07_dhti17a1nd.Models.TaiKhoan", b =>
                {
                    b.Navigation("KhachHang");
                });
#pragma warning restore 612, 618
        }
    }
}
