// ============================================================================
// HỌ VÀ TÊN: Lưu Đức Huy
// MÃ SINH VIÊN: 23274800002
// LỚP: DHTI17A2ND
// EMAIL: ldhuy.dhti17a2nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 2 - SẢN PHẨM, VOUCHER, HOÀN HÀNG & BÁO CÁO EXCEL/PDF
// NỘI DUNG: CRUD Quản lý mã giảm giá Voucher, số lượt sử dụng và thời hạn áp dụng
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Controllers
{
    [AdminAuthorize]
    public class VoucherController : Controller
    {
        private readonly AppDbContext _context;
        public VoucherController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? keyword)
        {
            var query = _context.Vouchers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(v => v.MaCode.Contains(keyword) || v.TenVoucher.Contains(keyword));

            ViewBag.Keyword = keyword;
            return View(await query.OrderByDescending(v => v.NgayBatDau).ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voucher v)
        {
            if (await _context.Vouchers.AnyAsync(x => x.MaCode == v.MaCode))
                ModelState.AddModelError("MaCode", "Mã voucher đã tồn tại.");
            if (v.NgayBatDau.Date > v.NgayKetThuc.Date)
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
            if (!ModelState.IsValid) return View(v);

            _context.Vouchers.Add(v);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Tạo voucher thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var v = await _context.Vouchers.FindAsync(id);
            if (v == null) return NotFound();
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Voucher v)
        {
            if (id != v.MaVoucher) return NotFound();

            if (await _context.Vouchers.AnyAsync(x => x.MaCode == v.MaCode && x.MaVoucher != id))
                ModelState.AddModelError("MaCode", "Mã voucher đã tồn tại.");
            if (!ModelState.IsValid) return View(v);

            _context.Vouchers.Update(v);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật voucher thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var v = await _context.Vouchers.Include(x => x.DonHangs).FirstOrDefaultAsync(x => x.MaVoucher == id);
            if (v == null) return NotFound();
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _context.Vouchers.Include(x => x.DonHangs).FirstOrDefaultAsync(x => x.MaVoucher == id);
            if (v == null) return Json(new { success = false, message = "Không tìm thấy." });
            if (v.DonHangs.Any()) return Json(new { success = false, message = "Không thể xóa vì đã có đơn hàng sử dụng." });

            _context.Vouchers.Remove(v);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa thành công!" });
        }
    }
}