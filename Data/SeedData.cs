// ============================================================================
// HỌ VÀ TÊN: Vương Quang Đạt
// MÃ SINH VIÊN: 23203100036
// LỚP: DHTI17A1ND
// EMAIL: vqdat.dhti17a1nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 3 - GIỎ HÀNG, ĐẶT HÀNG, THANH TOÁN VIETQR/COD & KHÁCH HÀNG
// NỘI DUNG: Khởi tạo dữ liệu mẫu tài khoản, danh mục, sản phẩm kèm 2 sản phẩm 5k-10k test VietQR thực tế
// ============================================================================

using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            if (await context.TaiKhoans.AnyAsync()) return;

            // ===== ADMIN =====
            var admin1 = new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau = PasswordHelper.HashPassword("admin123"),
                HoTen = "Quản Trị Viên",
                Email = "admin@uneti.edu.vn",
                VaiTro = "Admin",
                TrangThai = true
            };
            var admin2 = new TaiKhoan
            {
                TenDangNhap = "admin2",
                MatKhau = PasswordHelper.HashPassword("admin123"),
                HoTen = "Admin Phụ",
                Email = "admin2@uneti.edu.vn",
                VaiTro = "Admin",
                TrangThai = true
            };
            context.TaiKhoans.AddRange(admin1, admin2);

            // ===== LOẠI SẢN PHẨM =====
            var loaiSPs = new List<LoaiSanPham>
            {
                new() { TenLoai = "Điện thoại", MoTa = "Điện thoại di động", TrangThai = true },
                new() { TenLoai = "Laptop", MoTa = "Máy tính xách tay", TrangThai = true },
                new() { TenLoai = "Tablet", MoTa = "Máy tính bảng", TrangThai = true },
                new() { TenLoai = "Phụ kiện", MoTa = "Tai nghe, sạc, cáp", TrangThai = true },
                new() { TenLoai = "Đồng hồ thông minh", MoTa = "Smartwatch", TrangThai = true }
            };
            context.LoaiSanPhams.AddRange(loaiSPs);
            await context.SaveChangesAsync();

            // ===== 20 SẢN PHẨM (với ảnh) =====
            var sanPhams = new List<SanPham>
            {
                new() { TenSanPham = "iPhone 15 Pro Max", MaLoai = loaiSPs[0].MaLoai, ThuongHieu = "Apple", XuatXu = "Mỹ", DonGia = 29990000, SoLuongTon = 50, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/iphone15.jpg", TrangThai = true },
                new() { TenSanPham = "Samsung Galaxy S24 Ultra", MaLoai = loaiSPs[0].MaLoai, ThuongHieu = "Samsung", XuatXu = "Hàn Quốc", DonGia = 26990000, SoLuongTon = 40, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/samsung-s24.jpg", TrangThai = true },
                new() { TenSanPham = "Xiaomi 14 Pro", MaLoai = loaiSPs[0].MaLoai, ThuongHieu = "Xiaomi", XuatXu = "Trung Quốc", DonGia = 19990000, SoLuongTon = 35, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/xiaomi-14.jpg", TrangThai = true },
                new() { TenSanPham = "OPPO Find X7", MaLoai = loaiSPs[0].MaLoai, ThuongHieu = "OPPO", XuatXu = "Trung Quốc", DonGia = 18990000, SoLuongTon = 25, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/oppo-findx7.jpg", TrangThai = true },
                new() { TenSanPham = "MacBook Pro M3", MaLoai = loaiSPs[1].MaLoai, ThuongHieu = "Apple", XuatXu = "Mỹ", DonGia = 49990000, SoLuongTon = 20, ThoiGianBaoHanh = 24, HinhAnh = "/images/products/macbook-m3.jpg", TrangThai = true },
                new() { TenSanPham = "Dell XPS 15", MaLoai = loaiSPs[1].MaLoai, ThuongHieu = "Dell", XuatXu = "Mỹ", DonGia = 35990000, SoLuongTon = 15, ThoiGianBaoHanh = 24, HinhAnh = "/images/products/dell-xps15.jpg", TrangThai = true },
                new() { TenSanPham = "Asus ROG Strix G16", MaLoai = loaiSPs[1].MaLoai, ThuongHieu = "Asus", XuatXu = "Đài Loan", DonGia = 32990000, SoLuongTon = 18, ThoiGianBaoHanh = 24, HinhAnh = "/images/products/asus-rog.jpg", TrangThai = true },
                new() { TenSanPham = "HP Spectre x360", MaLoai = loaiSPs[1].MaLoai, ThuongHieu = "HP", XuatXu = "Mỹ", DonGia = 28990000, SoLuongTon = 12, ThoiGianBaoHanh = 24, HinhAnh = "/images/products/hp-spectre.jpg", TrangThai = true },
                new() { TenSanPham = "iPad Pro 12.9 M2", MaLoai = loaiSPs[2].MaLoai, ThuongHieu = "Apple", XuatXu = "Mỹ", DonGia = 25990000, SoLuongTon = 30, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/ipad-pro.jpg", TrangThai = true },
                new() { TenSanPham = "Samsung Galaxy Tab S9", MaLoai = loaiSPs[2].MaLoai, ThuongHieu = "Samsung", XuatXu = "Hàn Quốc", DonGia = 18990000, SoLuongTon = 22, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/tab-s9.jpg", TrangThai = true },
                new() { TenSanPham = "Xiaomi Pad 6", MaLoai = loaiSPs[2].MaLoai, ThuongHieu = "Xiaomi", XuatXu = "Trung Quốc", DonGia = 8990000, SoLuongTon = 45, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/xiaomi-pad6.jpg", TrangThai = true },
                new() { TenSanPham = "AirPods Pro 2", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "Apple", XuatXu = "Mỹ", DonGia = 5990000, SoLuongTon = 100, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/airpods-pro2.jpg", TrangThai = true },
                new() { TenSanPham = "Samsung Galaxy Buds 2 Pro", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "Samsung", XuatXu = "Hàn Quốc", DonGia = 4490000, SoLuongTon = 80, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/buds2pro.jpg", TrangThai = true },
                new() { TenSanPham = "Sạc nhanh Anker 65W", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "Anker", XuatXu = "Trung Quốc", DonGia = 890000, SoLuongTon = 200, ThoiGianBaoHanh = 6, HinhAnh = "/images/products/anker-65w.jpg", TrangThai = true },
                new() { TenSanPham = "Cáp Type-C Baseus", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "Baseus", XuatXu = "Trung Quốc", DonGia = 150000, SoLuongTon = 500, ThoiGianBaoHanh = 3, HinhAnh = "/images/products/cap-baseus.jpg", TrangThai = true },
                new() { TenSanPham = "Apple Watch Series 9", MaLoai = loaiSPs[4].MaLoai, ThuongHieu = "Apple", XuatXu = "Mỹ", DonGia = 10990000, SoLuongTon = 40, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/watch-s9.jpg", TrangThai = true },
                new() { TenSanPham = "Samsung Galaxy Watch 6", MaLoai = loaiSPs[4].MaLoai, ThuongHieu = "Samsung", XuatXu = "Hàn Quốc", DonGia = 7990000, SoLuongTon = 35, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/watch6.jpg", TrangThai = true },
                new() { TenSanPham = "Garmin Venu 3", MaLoai = loaiSPs[4].MaLoai, ThuongHieu = "Garmin", XuatXu = "Mỹ", DonGia = 9990000, SoLuongTon = 25, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/garmin-venu3.jpg", TrangThai = true },
                new() { TenSanPham = "Chuột Logitech MX Master 3S", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "Logitech", XuatXu = "Thụy Sĩ", DonGia = 2490000, SoLuongTon = 60, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/logitech-mx.jpg", TrangThai = true },
                new() { TenSanPham = "Bàn phím cơ Keychron K8", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "Keychron", XuatXu = "Trung Quốc", DonGia = 2290000, SoLuongTon = 55, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/keychron-k8.jpg", TrangThai = true },
                new() { TenSanPham = "Cáp sạc Mini Type-C (Test VietQR 5K)", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "LioShop", XuatXu = "Việt Nam", DonGia = 5000, SoLuongTon = 999, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/cap-typec.jpg", TrangThai = true },
                new() { TenSanPham = "Jack chuyển đổi Audio (Test VietQR 10K)", MaLoai = loaiSPs[3].MaLoai, ThuongHieu = "LioShop", XuatXu = "Việt Nam", DonGia = 10000, SoLuongTon = 999, ThoiGianBaoHanh = 12, HinhAnh = "/images/products/jack-audio.jpg", TrangThai = true }
            };
            context.SanPhams.AddRange(sanPhams);
            await context.SaveChangesAsync();

            // ===== 15 KHÁCH HÀNG =====
            var hoTens = new[] { "Nguyễn Văn An", "Trần Thị Bình", "Lê Văn Cường", "Phạm Thị Dung", "Hoàng Văn Em",
                                 "Vũ Thị Phương", "Đặng Văn Giang", "Bùi Thị Hoa", "Ngô Văn Inh", "Dương Thị Kim",
                                 "Đỗ Văn Long", "Hồ Thị Mai", "Lý Văn Nam", "Trịnh Thị Oanh", "Phan Văn Phúc" };

            var khachHangs = new List<KhachHang>();
            for (int i = 0; i < 15; i++)
            {
                var tk = new TaiKhoan
                {
                    TenDangNhap = $"kh{i + 1:D2}",
                    MatKhau = PasswordHelper.HashPassword("khach123"),
                    HoTen = hoTens[i],
                    Email = $"kh{i + 1:D2}@gmail.com",
                    VaiTro = "KhachHang",
                    TrangThai = true
                };
                context.TaiKhoans.Add(tk);
                await context.SaveChangesAsync();

                var kh = new KhachHang
                {
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = hoTens[i],
                    NgaySinh = new DateTime(1990 + i % 10, (i % 12) + 1, (i % 28) + 1),
                    GioiTinh = i % 2 == 0 ? "Nam" : "Nữ",
                    SoDienThoai = $"09{i + 1:D8}",
                    Email = $"kh{i + 1:D2}@gmail.com",
                    DiaChi = $"{i + 1} Đường ABC, Quận {i % 12 + 1}, Hà Nội",
                    NgayDangKy = DateTime.Now.AddDays(-60 + i * 3),
                    TrangThai = true
                };
                khachHangs.Add(kh);
                context.KhachHangs.Add(kh);
            }
            await context.SaveChangesAsync();

            // ===== VOUCHER =====
            var vouchers = new List<Voucher>
            {
                new() { MaCode = "GIAM10", TenVoucher = "Giảm 10%", LoaiGiamGia = "PhanTram", GiaTriGiam = 10, GiamToiDa = 500000, DonHangToiThieu = 1000000, NgayBatDau = DateTime.Now.AddDays(-10), NgayKetThuc = DateTime.Now.AddDays(30), SoLuong = 100, TrangThai = true },
                new() { MaCode = "GIAM20", TenVoucher = "Giảm 20%", LoaiGiamGia = "PhanTram", GiaTriGiam = 20, GiamToiDa = 1000000, DonHangToiThieu = 5000000, NgayBatDau = DateTime.Now.AddDays(-5), NgayKetThuc = DateTime.Now.AddDays(30), SoLuong = 50, TrangThai = true },
                new() { MaCode = "GIAM500K", TenVoucher = "Giảm 500K", LoaiGiamGia = "SoTien", GiaTriGiam = 500000, DonHangToiThieu = 3000000, NgayBatDau = DateTime.Now.AddDays(-3), NgayKetThuc = DateTime.Now.AddDays(20), SoLuong = 200, TrangThai = true },
                new() { MaCode = "GIAM1TR", TenVoucher = "Giảm 1 Triệu", LoaiGiamGia = "SoTien", GiaTriGiam = 1000000, DonHangToiThieu = 10000000, NgayBatDau = DateTime.Now, NgayKetThuc = DateTime.Now.AddDays(60), SoLuong = 30, TrangThai = true },
                new() { MaCode = "WELCOME", TenVoucher = "Chào mừng", LoaiGiamGia = "PhanTram", GiaTriGiam = 5, GiamToiDa = 200000, DonHangToiThieu = 0, NgayBatDau = DateTime.Now.AddDays(-30), NgayKetThuc = DateTime.Now.AddDays(90), SoLuong = 1000, TrangThai = true }
            };
            context.Vouchers.AddRange(vouchers);
            await context.SaveChangesAsync();

            // ===== 20 ĐƠN HÀNG =====
            var random = new Random(42);
            var trangThais = new[] { "ChoXacNhan", "DaXacNhan", "DangGiao", "DaHoanThanh", "DaHuy" };
            for (int i = 0; i < 20; i++)
            {
                var kh = khachHangs[i % khachHangs.Count];
                var soSanPham = random.Next(1, 4);
                var chiTiets = new List<ChiTietDonHang>();
                decimal tongTien = 0;

                for (int j = 0; j < soSanPham; j++)
                {
                    var sp = sanPhams[random.Next(sanPhams.Count)];
                    var soLuong = random.Next(1, 3);
                    var thanhTien = sp.DonGia * soLuong;
                    tongTien += thanhTien;

                    chiTiets.Add(new ChiTietDonHang
                    {
                        MaSanPham = sp.MaSanPham,
                        SoLuong = soLuong,
                        DonGia = sp.DonGia,
                        ThanhTien = thanhTien
                    });
                }

                var donHang = new DonHang
                {
                    MaKhachHang = kh.MaKhachHang,
                    NgayDat = DateTime.Now.AddDays(-random.Next(1, 60)),
                    DiaChiGiaoHang = kh.DiaChi ?? "Hà Nội",
                    TongTien = tongTien,
                    TrangThai = trangThais[random.Next(trangThais.Length)],
                    ChiTietDonHangs = chiTiets
                };
                context.DonHangs.Add(donHang);
            }
            await context.SaveChangesAsync();
        }
    }
}