// Họ và tên: Pham Thai Hoang
// Mã sinh viên: 23203100039
// Nội dung thực hiện: Dashboard và thống kê
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Helpers;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly AppDbContext _context;

        public ThongKeController(AppDbContext context)
        {
            _context = context;
        }

        private bool LaAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vm = new DashboardViewModel
            {
                TongLoaiSanPham = await _context.LoaiSanPhams.CountAsync(),
                TongSanPham = await _context.SanPhams.CountAsync(),
                TongKhachHang = await _context.KhachHangs.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.ChoXacNhan),
                DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.DangGiao),
                DonHoanThanh = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.DaHoanThanh),
                SanPhamDangKinhDoanh = await _context.SanPhams.CountAsync(s => s.TrangThai == "Đang kinh doanh")
            };

            return View(vm);
        }
        public async Task<IActionResult> ThongKe()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vm = new ThongKeViewModel();

            // 1. Số sản phẩm theo từng loại (đếm bằng Count)
            vm.SanPhamTheoLoai = await _context.LoaiSanPhams
                .Select(l => new ThongKeItem
                {
                    Ten = l.TenLoai,
                    SoLuong = _context.SanPhams.Count(s => s.MaLoai == l.MaLoai)
                })
                .ToListAsync();

            // 2. Số đơn hàng theo từng trạng thái (GroupBy + Count)
            vm.DonTheoTrangThai = await _context.DonHangs
                .GroupBy(d => d.TrangThai)
                .Select(g => new ThongKeItem { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 3. Số lượng bán theo từng sản phẩm (chỉ tính đơn Đã hoàn thành)
            var banTheoSanPham = await _context.DonHangs
                .Where(d => d.TrangThai == TrangThaiDonHang.DaHoanThanh)
                .SelectMany(d => d.ChiTietDonHangs)
                .GroupBy(ct => new { ct.SanPham.MaSanPham, ct.SanPham.TenSanPham })
                .Select(g => new ThongKeItem { Ten = g.Key.TenSanPham, SoLuong = g.Sum(ct => ct.SoLuong) })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();
            vm.SoLuongBanTheoSanPham = banTheoSanPham;

            // 4. Sản phẩm bán nhiều nhất: dòng đầu của danh sách đã sắp giảm dần
            vm.SanPhamBanChayNhat = banTheoSanPham.FirstOrDefault();

            // 5. Số đơn hàng theo từng khách hàng
            vm.DonTheoKhachHang = await _context.DonHangs
                .GroupBy(d => new { d.KhachHang.MaKhachHang, d.KhachHang.HoTen })
                .Select(g => new ThongKeItem { Ten = g.Key.HoTen, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 6. Doanh thu theo tháng (chỉ tính đơn Đã hoàn thành)
            vm.DoanhThuTheoThang = await _context.DonHangs
                .Where(d => d.TrangThai == TrangThaiDonHang.DaHoanThanh)
                .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
                .Select(g => new DoanhThuThang
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    DoanhThu = g.Sum(d => d.TongTien)
                })
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .ToListAsync();

            return View(vm);
        }
    }
}