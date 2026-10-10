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
    }
}