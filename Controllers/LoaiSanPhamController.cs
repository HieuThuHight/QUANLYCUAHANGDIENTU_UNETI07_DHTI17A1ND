// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: CRUD LoaiSanPham

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Controllers
{
    [AdminAuthorize]
    public class LoaiSanPhamController : Controller
    {
        private readonly AppDbContext _context;
        public LoaiSanPhamController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? keyword)
        {
            var query = _context.LoaiSanPhams.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(l => l.TenLoai.Contains(keyword));

            ViewBag.Keyword = keyword;
            return View(await query.OrderBy(l => l.TenLoai).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var loai = await _context.LoaiSanPhams
                .Include(l => l.SanPhams)
                .FirstOrDefaultAsync(l => l.MaLoai == id);
            if (loai == null) return NotFound();
            return View(loai);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiSanPham loai)
        {
            if (await _context.LoaiSanPhams.AnyAsync(l => l.TenLoai == loai.TenLoai))
                ModelState.AddModelError("TenLoai", "Tên loại đã tồn tại.");

            if (!ModelState.IsValid) return View(loai);

            _context.Add(loai);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm loại sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var loai = await _context.LoaiSanPhams.FindAsync(id);
            if (loai == null) return NotFound();
            return View(loai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiSanPham loai)
        {
            if (id != loai.MaLoai) return NotFound();

            if (await _context.LoaiSanPhams.AnyAsync(l => l.TenLoai == loai.TenLoai && l.MaLoai != id))
                ModelState.AddModelError("TenLoai", "Tên loại đã tồn tại.");

            if (!ModelState.IsValid) return View(loai);

            _context.Update(loai);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var loai = await _context.LoaiSanPhams
                .Include(l => l.SanPhams)
                .FirstOrDefaultAsync(l => l.MaLoai == id);

            if (loai == null) return Json(new { success = false, message = "Không tìm thấy." });
            if (loai.SanPhams.Any())
                return Json(new { success = false, message = "Không thể xóa vì có sản phẩm liên quan." });

            _context.LoaiSanPhams.Remove(loai);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa thành công!" });
        }
    }
}