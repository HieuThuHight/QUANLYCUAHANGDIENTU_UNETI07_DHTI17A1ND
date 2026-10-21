// Họ và tên: Vương Quang Đạt
// Mã sinh viên: 23203100036
// Nội dung thực hiện: Quan ly khach hang va ho so ca nhan

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Controllers
{
    // Quản lý khách hàng - CHỈ ADMIN
    [AdminAuthorize]
    public class KhachHangController : Controller
    {
        private readonly AppDbContext _context;
        public KhachHangController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? keyword, int pageIndex = 1)
        {
            var query = _context.KhachHangs.Include(k => k.TaiKhoan).AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(k => k.HoTen.Contains(keyword) || k.SoDienThoai.Contains(keyword));

            ViewBag.Keyword = keyword;
            var list = await PaginatedList<KhachHang>.CreateAsync(query.OrderBy(k => k.HoTen), pageIndex, 10);
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var kh = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.DonHangs)
                .FirstOrDefaultAsync(k => k.MaKhachHang == id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var kh = await _context.KhachHangs.FindAsync(id);
            if (kh == null) return Json(new { success = false });
            kh.TrangThai = !kh.TrangThai;
            await _context.SaveChangesAsync();
            return Json(new { success = true, trangThai = kh.TrangThai });
        }
    }

    // Hồ sơ cá nhân - CHỈ KHÁCH
    [KhachHangAuthorize]
    public class HoSoController : Controller
    {
        private readonly AppDbContext _context;
        public HoSoController(AppDbContext context) => _context = context;

        public async Task<IActionResult> HoSo()
        {
            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var kh = await _context.KhachHangs.FindAsync(maKH.Value);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSo(KhachHang model)
        {
            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var kh = await _context.KhachHangs.FindAsync(maKH.Value);
            if (kh == null) return NotFound();

            kh.HoTen = model.HoTen;
            kh.NgaySinh = model.NgaySinh;
            kh.GioiTinh = model.GioiTinh;
            kh.SoDienThoai = model.SoDienThoai;
            kh.DiaChi = model.DiaChi;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction(nameof(HoSo));
        }
    }
}
