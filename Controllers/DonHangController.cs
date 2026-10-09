// Họ và tên: Pham Thai Hoang
// Mã sinh viên: 23203100039
// Nội dung thực hiện: Quản lý đơn hàng, xử lý trạng thái
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Helpers;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class DonHangController : Controller
    {
        private readonly AppDbContext _context;   // [kiểm tra] tên DbContext, có thể cần thêm using ...Data;

        public DonHangController(AppDbContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền Admin ở phía Server (Session do Module 1 lưu)
        private bool LaAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";   // [kiểm tra] tên key và giá trị
        }

        public async Task<IActionResult> Index(string? tenKhachHang, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var query = _context.DonHangs.Include(d => d.KhachHang).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tenKhachHang))
                query = query.Where(d => d.KhachHang.HoTen.Contains(tenKhachHang));

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            if (tuNgay.HasValue)
                query = query.Where(d => d.NgayDat >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                query = query.Where(d => d.NgayDat < denNgay.Value.Date.AddDays(1));

            ViewBag.TenKhachHang = tenKhachHang;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");

            var danhSach = await query.OrderByDescending(d => d.NgayDat).ToListAsync();
            return View(danhSach);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var don = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs)      // [kiểm tra] tên navigation
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (don == null) return NotFound();
            return View(don);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuyenTrangThai(int id, string trangThaiMoi)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var don = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (don == null) return NotFound();

            // 1. Kiểm tra luồng trạng thái hợp lệ
            if (!TrangThaiDonHang.ChuyenDuoc(don.TrangThai, trangThaiMoi))
            {
                TempData["Loi"] = $"Không thể chuyển từ '{don.TrangThai}' sang '{trangThaiMoi}'.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // 2. Xác nhận: kiểm tra lại tồn kho rồi trừ tồn
            if (trangThaiMoi == TrangThaiDonHang.DaXacNhan)
            {
                foreach (var ct in don.ChiTietDonHangs)
                {
                    if (ct.SanPham.SoLuongTon < ct.SoLuong)
                    {
                        TempData["Loi"] = $"Sản phẩm '{ct.SanPham.TenSanPham}' không đủ tồn kho.";
                        return RedirectToAction(nameof(Details), new { id });
                    }
                }
                foreach (var ct in don.ChiTietDonHangs)
                    ct.SanPham.SoLuongTon -= ct.SoLuong;
            }
            // 3. Hủy đơn đã xác nhận: hoàn lại tồn kho (đơn chưa xác nhận chưa trừ nên không hoàn)
            else if (trangThaiMoi == TrangThaiDonHang.DaHuy && don.TrangThai == TrangThaiDonHang.DaXacNhan)
            {
                foreach (var ct in don.ChiTietDonHangs)
                    ct.SanPham.SoLuongTon += ct.SoLuong;
            }

            don.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã chuyển đơn #{id} sang '{trangThaiMoi}'.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}