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

        public async Task<IActionResult> Index()
        {
            // Đếm số lượng bằng LINQ
            var vm = new DashboardViewModel
            {
                TongLoaiSanPham = await _context.LoaiSanPhams.CountAsync(),
                TongSanPham = await _context.SanPhams.CountAsync(),
                TongKhachHang = await _context.KhachHangs.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThai == "Chờ xác nhận"),
                DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThai == "Đang giao"),
                DonHoanThanh = await _context.DonHangs.CountAsync(d => d.TrangThai == "Đã hoàn thành"),
                SanPhamDangKinhDoanh = await _context.SanPhams.CountAsync(s => s.TrangThai == "Đang kinh doanh"),

                // Lấy 20 sản phẩm + join Loại
                DanhSachSanPham = await _context.SanPhams
                    .Include(s => s.LoaiSanPham)
                    .OrderBy(s => s.MaSanPham)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}

