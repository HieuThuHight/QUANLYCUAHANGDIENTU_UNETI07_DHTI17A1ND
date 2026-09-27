using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? tuKhoa)
        {
            if (!LaAdmin()) return Forbid();
            var truyVan = _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                truyVan = truyVan.Where(k =>
                    k.HoTen.Contains(tuKhoa) ||
                    (k.SoDienThoai != null && k.SoDienThoai.Contains(tuKhoa)) ||
                    (k.Email != null && k.Email.Contains(tuKhoa)));
            }
            ViewBag.TuKhoa = tuKhoa;
            var danhSach = await truyVan
                .OrderBy(k => k.HoTen)
                .ToListAsync();
            return View(danhSach);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (!LaAdmin()) return Forbid();
            var khachHang = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.DonHangs)
                .FirstOrDefaultAsync(k => k.MaKhachHang == id);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, bool trangThai)
        {
            if (!LaAdmin()) return Forbid();
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null) return NotFound();
            khachHang.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = trangThai
                ? $"Đã mở khóa khách hàng '{khachHang.HoTen}'."
                : $"Đã khóa khách hàng '{khachHang.HoTen}'.";
            return RedirectToAction(nameof(Details), new { id });
        }
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var maKhachHang = LayMaKhachHangDangDangNhap();
            if (maKhachHang == null) return RedirectToAction("DangNhap", "TaiKhoan");
            var khachHang = await _context.KhachHangs.FindAsync(maKhachHang.Value);
            if (khachHang == null) return NotFound();
            return View(khachHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinCaNhan(
            [Bind("HoTen,NgaySinh,GioiTinh,SoDienThoai,Email,DiaChi")] KhachHang duLieuCapNhat)
        {
            var maKhachHang = LayMaKhachHangDangDangNhap();
            if (maKhachHang == null) return RedirectToAction("DangNhap", "TaiKhoan");
            var khachHang = await _context.KhachHangs.FindAsync(maKhachHang.Value);
            if (khachHang == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(khachHang);
            }
            khachHang.HoTen = duLieuCapNhat.HoTen;
            khachHang.NgaySinh = duLieuCapNhat.NgaySinh;
            khachHang.GioiTinh = duLieuCapNhat.GioiTinh;
            khachHang.SoDienThoai = duLieuCapNhat.SoDienThoai;
            khachHang.Email = duLieuCapNhat.Email;
            khachHang.DiaChi = duLieuCapNhat.DiaChi;
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Cập nhật thông tin cá nhân thành công.";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
        private bool LaAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }
        private int? LayMaKhachHangDangDangNhap()
        {
            return HttpContext.Session.GetInt32("MaKhachHang");
        }
    }
}
