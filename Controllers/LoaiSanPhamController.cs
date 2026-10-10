// Họ và tên: [Tên SV1]
// Mã sinh viên: [Mã SV1]
// Nội dung thực hiện: Module 1 - CRUD loại sản phẩm

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Filters;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    [YeuCauAdmin]
    public class LoaiSanPhamController : Controller
    {
        private readonly AppDbContext _context;

        public LoaiSanPhamController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. DANH SÁCH
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var ds = await _context.LoaiSanPhams
                .OrderBy(l => l.MaLoai)
                .ToListAsync();

            // Đếm số sản phẩm của mỗi loại (để hiện trên bảng)
            ViewBag.SoSanPham = await _context.SanPhams
                .GroupBy(s => s.MaLoai)
                .Select(g => new { MaLoai = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.MaLoai, x => x.SoLuong);

            return View(ds);
        }

        // ============================================================
        // 2. CHI TIẾT
        // ============================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loai = await _context.LoaiSanPhams
                .FirstOrDefaultAsync(l => l.MaLoai == id);

            if (loai == null) return NotFound();

            // Đếm số SP liên quan (hiển thị ở trang chi tiết)
            ViewBag.SoSanPham = await _context.SanPhams
                .CountAsync(s => s.MaLoai == id);

            return View(loai);
        }

        // ============================================================
        // 3. THÊM MỚI
        // ============================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiSanPham model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra trùng tên
            var trungTen = await _context.LoaiSanPhams
                .AnyAsync(l => l.TenLoai == model.TenLoai);

            if (trungTen)
            {
                ModelState.AddModelError("TenLoai", "Tên loại sản phẩm đã tồn tại!");
                return View(model);
            }

            _context.LoaiSanPhams.Add(model);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "✅ Thêm loại sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 4. SỬA
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loai = await _context.LoaiSanPhams.FindAsync(id);
            if (loai == null) return NotFound();

            return View(loai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiSanPham model)
        {
            if (id != model.MaLoai) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra trùng tên (trừ chính nó)
            var trungTen = await _context.LoaiSanPhams
                .AnyAsync(l => l.TenLoai == model.TenLoai && l.MaLoai != id);

            if (trungTen)
            {
                ModelState.AddModelError("TenLoai", "Tên loại sản phẩm đã tồn tại!");
                return View(model);
            }

            try
            {
                _context.LoaiSanPhams.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = "✅ Cập nhật loại sản phẩm thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LoaiSanPhams.AnyAsync(l => l.MaLoai == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 5. XÓA
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loai = await _context.LoaiSanPhams
                .FirstOrDefaultAsync(l => l.MaLoai == id);

            if (loai == null) return NotFound();

            // Danh sách SP liên quan để hiển thị cảnh báo
            ViewBag.DanhSachSanPham = await _context.SanPhams
                .Where(s => s.MaLoai == id)
                .Select(s => new { s.MaSanPham, s.TenSanPham })
                .ToListAsync();

            ViewBag.CoSanPham = ViewBag.DanhSachSanPham.Count > 0;

            return View(loai);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loai = await _context.LoaiSanPhams.FindAsync(id);
            if (loai == null) return NotFound();

            // ⚠️ Chặn xóa nếu còn sản phẩm liên quan
            var coSanPham = await _context.SanPhams.AnyAsync(s => s.MaLoai == id);
            if (coSanPham)
            {
                TempData["Loi"] = "❌ Không thể xóa! Loại sản phẩm này đang có sản phẩm liên quan.";
                return RedirectToAction(nameof(Index));
            }

            _context.LoaiSanPhams.Remove(loai);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "✅ Xóa loại sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 6. CẬP NHẬT TRẠNG THÁI (nhanh)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id)
        {
            var loai = await _context.LoaiSanPhams.FindAsync(id);
            if (loai == null) return NotFound();

            loai.TrangThai = !loai.TrangThai;
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "✅ Cập nhật trạng thái thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}