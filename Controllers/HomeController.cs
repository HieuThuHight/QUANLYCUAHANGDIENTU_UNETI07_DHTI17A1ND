// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Trang chu - redirect theo vai tro, hien thi san pham noi bat

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;

namespace QuanLyCuaHangDienTu.Controllers
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
            var sanPhams = await _context.SanPhams
                .Include(s => s.LoaiSanPham)
                .Where(s => s.TrangThai)
                .OrderByDescending(s => s.MaSanPham)
                .Take(8)
                .ToListAsync();

            return View(sanPhams);
        }

        public IActionResult Error() => View();
    }
}