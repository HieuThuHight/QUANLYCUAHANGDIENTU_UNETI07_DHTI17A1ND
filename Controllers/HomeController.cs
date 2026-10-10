using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
using System.Diagnostics;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // ===== GET: / (Trang chủ trưng bày sản phẩm) =====
        public async Task<IActionResult> Index()
        {
            // Nếu đã đăng nhập và là Admin → vào Dashboard
            if (HttpContext.Session.GetString("VaiTro") == "Admin")
            {
                return RedirectToAction("Dashboard");
            }

            // Khách vãng lai / Khách hàng → trưng bày sản phẩm
            var sanPhams = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .Where(s => s.TrangThai == "Đang kinh doanh")
                .OrderByDescending(s => s.MaSanPham)
                .Take(12)                                       // Lấy 12 SP mới nhất
                .ToListAsync();

            return View(sanPhams);
        }

        // ===== GET: /Home/Dashboard (Chỉ Admin — Module 4) =====
        public async Task<IActionResult> Dashboard()
        {
            // Kiểm tra quyền Admin ở Server
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }

            ViewBag.TongLoaiSanPham = await _context.LoaiSanPhams.CountAsync();
            ViewBag.TongSanPham = await _context.SanPhams.CountAsync();
            ViewBag.TongKhachHang = await _context.KhachHangs.CountAsync();
            ViewBag.TongDonHang = await _context.DonHangs.CountAsync();
            ViewBag.DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThai == "Chờ xác nhận");
            ViewBag.DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThai == "Đang giao");
            ViewBag.DonHoanThanh = await _context.DonHangs.CountAsync(d => d.TrangThai == "Đã hoàn thành");
            ViewBag.SanPhamDangKinhDoanh = await _context.SanPhams.CountAsync(s => s.TrangThai == "Đang kinh doanh");

            return View();
        }

        // ===== GET: /Home/Privacy =====
        public IActionResult Privacy()
        {
            return View();
        }
    }
}

