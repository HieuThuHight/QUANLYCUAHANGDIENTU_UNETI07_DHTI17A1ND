// Họ và tên: Phạm Thái Hoàng
// Mã sinh viên: 23203100039
// Nội dung thực hiện: Quan ly don hang, xu ly trang thai

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;
using QuanLyCuaHangDienTu.Services;
using System.Data;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class DonHangController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(AppDbContext context, EmailService emailService, ILogger<DonHangController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // ===== KHÁCH XEM ĐƠN CỦA MÌNH =====
        [KhachHangAuthorize]
        public async Task<IActionResult> Index(string? trangThai)
        {
            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var query = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .Include(d => d.Voucher)
                .Where(d => d.MaKhachHang == maKH.Value);

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            ViewBag.TrangThai = trangThai;
            return View(await query.OrderByDescending(d => d.NgayDat).ToListAsync());
        }

        [KhachHangAuthorize]
        public async Task<IActionResult> Details(int id)
        {
            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var dh = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs).ThenInclude(c => c.SanPham)
                .Include(d => d.Voucher)
                .Include(d => d.KhachHang)
                .Include(d => d.YeuCauHoanHangs)
                .FirstOrDefaultAsync(d => d.MaDonHang == id && d.MaKhachHang == maKH.Value);

            if (dh == null) return NotFound();
            return View(dh);
        }

        // ===== ADMIN QUẢN LÝ =====
        [AdminAuthorize]
        public async Task<IActionResult> QuanLy(string? keyword, string? trangThai, DateTime? tuNgay, DateTime? denNgay, int pageIndex = 1)
        {
            var query = _context.DonHangs.Include(d => d.KhachHang).Include(d => d.Voucher).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(d => d.KhachHang!.HoTen.Contains(keyword));
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);
            if (tuNgay.HasValue)
                query = query.Where(d => d.NgayDat.Date >= tuNgay.Value.Date);
            if (denNgay.HasValue)
                query = query.Where(d => d.NgayDat.Date <= denNgay.Value.Date);

            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");

            var list = await PaginatedList<DonHang>.CreateAsync(query.OrderByDescending(d => d.NgayDat), pageIndex, 10);
            return View(list);
        }

        [AdminAuthorize]
        public async Task<IActionResult> ChiTietAdmin(int id)
        {
            var dh = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs).ThenInclude(c => c.SanPham)
                .Include(d => d.Voucher)
                .Include(d => d.YeuCauHoanHangs)
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (dh == null) return NotFound();
            return View(dh);
        }

        // ===== CHUYỂN TRẠNG THÁI - CÓ TRANSACTION =====
        [HttpPost]
        [AdminAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChuyenTrangThai(int id, string trangThai)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var dh = await _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .Include(d => d.KhachHang)
                    .Include(d => d.Voucher)
                    .FirstOrDefaultAsync(d => d.MaDonHang == id);

                if (dh == null) { await transaction.RollbackAsync(); return Json(new { success = false, message = "Không tìm thấy." }); }

                var hopLe = new Dictionary<string, string[]>
                {
                    ["ChoXacNhan"] = new[] { "DaXacNhan", "DaHuy" },
                    ["DaXacNhan"] = new[] { "DangGiao", "DaHuy" },
                    ["DangGiao"] = new[] { "DaHoanThanh" },
                    ["DaHoanThanh"] = new[] { "YeuCauHoanHang" },
                    ["DaHuy"] = Array.Empty<string>(),
                    ["YeuCauHoanHang"] = new[] { "DaHoanHang", "DaHoanThanh" },
                    ["DaHoanHang"] = Array.Empty<string>()
                };

                if (!hopLe.ContainsKey(dh.TrangThai) || !hopLe[dh.TrangThai].Contains(trangThai))
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Không thể chuyển sang trạng thái này." });
                }

                var trangThaiCu = dh.TrangThai;

                // Trừ kho khi xác nhận
                if (trangThai == "DaXacNhan" && dh.TrangThai == "ChoXacNhan")
                {
                    foreach (var ct in dh.ChiTietDonHangs)
                    {
                        var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
                        if (sp == null || sp.SoLuongTon < ct.SoLuong)
                        {
                            await transaction.RollbackAsync();
                            return Json(new { success = false, message = $"SP '{sp?.TenSanPham}' không đủ tồn kho." });
                        }
                    }
                    foreach (var ct in dh.ChiTietDonHangs)
                    {
                        var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
                        sp!.SoLuongTon -= ct.SoLuong;
                    }
                    if (dh.Voucher != null)
                    {
                        if (dh.Voucher.DaSuDung >= dh.Voucher.SoLuong)
                        {
                            await transaction.RollbackAsync();
                            return Json(new { success = false, message = "Voucher đã hết lượt." });
                        }
                        dh.Voucher.DaSuDung++;
                    }
                }

                // Hoàn kho khi hủy
                if (trangThai == "DaHuy" && (dh.TrangThai == "DaXacNhan" || dh.TrangThai == "DangGiao"))
                {
                    foreach (var ct in dh.ChiTietDonHangs)
                    {
                        var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
                        if (sp != null) sp.SoLuongTon += ct.SoLuong;
                    }
                    if (dh.Voucher != null && dh.Voucher.DaSuDung > 0) dh.Voucher.DaSuDung--;
                }

                dh.TrangThai = trangThai;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (dh.KhachHang != null && trangThaiCu != trangThai)
                {
                    _ = Task.Run(async () =>
                    {
                        try { await _emailService.GuiEmailCapNhatTrangThai(dh, dh.KhachHang.Email, trangThai); }
                        catch (Exception ex) { _logger.LogError(ex, "Lỗi gửi email"); }
                    });
                }

                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi chuyển trạng thái");
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }
    }
}