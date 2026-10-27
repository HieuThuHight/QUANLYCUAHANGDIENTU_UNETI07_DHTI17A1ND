// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Xu ly yeu cau hoan hang

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Models.Entities;
using QuanLyCuaHangDienTu.Services;
using System.Data;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class HoanHangController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<HoanHangController> _logger;

        public HoanHangController(AppDbContext context, EmailService emailService, ILogger<HoanHangController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // KHÁCH YÊU CẦU
        [KhachHangAuthorize]
        public async Task<IActionResult> YeuCau(int maDonHang)
        {
            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var dh = await _context.DonHangs.FirstOrDefaultAsync(d => d.MaDonHang == maDonHang && d.MaKhachHang == maKH.Value);
            if (dh == null || dh.TrangThai != "DaHoanThanh")
            {
                TempData["Error"] = "Chỉ có thể yêu cầu hoàn hàng cho đơn đã hoàn thành.";
                return RedirectToAction("Index", "DonHang");
            }

            ViewBag.MaDonHang = maDonHang;
            ViewBag.TongTien = dh.TongTien;
            return View();
        }

        [HttpPost]
        [KhachHangAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YeuCau(YeuCauHoanHang yc)
        {
            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var dh = await _context.DonHangs.FirstOrDefaultAsync(d => d.MaDonHang == yc.MaDonHang && d.MaKhachHang == maKH.Value);
            if (dh == null) return NotFound();

            yc.NgayYeuCau = DateTime.Now;
            yc.TrangThai = "ChoDuyet";
            yc.SoTienHoan = dh.TongTien;

            _context.YeuCauHoanHangs.Add(yc);
            dh.TrangThai = "YeuCauHoanHang";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gửi yêu cầu hoàn hàng.";
            return RedirectToAction("Index", "DonHang");
        }

        // ADMIN QUẢN LÝ
        [AdminAuthorize]
        public IActionResult Index() => RedirectToAction(nameof(QuanLy));

        [AdminAuthorize]
        public async Task<IActionResult> QuanLy(string? trangThai)
        {
            var query = _context.YeuCauHoanHangs.Include(y => y.DonHang).ThenInclude(d => d!.KhachHang).AsQueryable();
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(y => y.TrangThai == trangThai);

            ViewBag.TrangThai = trangThai;
            return View(await query.OrderByDescending(y => y.NgayYeuCau).ToListAsync());
        }

        [AdminAuthorize]
        public async Task<IActionResult> ChiTiet(int id)
        {
            var yc = await _context.YeuCauHoanHangs
                .Include(y => y.DonHang).ThenInclude(d => d!.KhachHang)
                .Include(y => y.DonHang).ThenInclude(d => d!.ChiTietDonHangs).ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(y => y.MaYeuCau == id);

            if (yc == null) return NotFound();
            return View(yc);
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> XuLy(int id, string trangThai, string? phanHoi)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var yc = await _context.YeuCauHoanHangs
                    .Include(y => y.DonHang).ThenInclude(d => d!.KhachHang)
                    .Include(y => y.DonHang).ThenInclude(d => d!.ChiTietDonHangs)
                    .FirstOrDefaultAsync(y => y.MaYeuCau == id);

                if (yc == null) { await transaction.RollbackAsync(); return Json(new { success = false }); }
                if (yc.TrangThai != "ChoDuyet") { await transaction.RollbackAsync(); return Json(new { success = false, message = "Đã xử lý." }); }

                yc.TrangThai = trangThai;
                yc.PhanHoiAdmin = phanHoi;
                yc.NgayXuLy = DateTime.Now;

                if (trangThai == "DaDuyet" || trangThai == "DaHoanTien")
                {
                    var dh = yc.DonHang!;
                    dh.TrangThai = "DaHoanHang";

                    foreach (var ct in dh.ChiTietDonHangs)
                    {
                        var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
                        if (sp != null) sp.SoLuongTon += ct.SoLuong;
                    }
                    yc.SoTienHoan = dh.TongTien;
                }
                else if (trangThai == "TuChoi")
                {
                    yc.DonHang!.TrangThai = "DaHoanThanh";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (yc.DonHang?.KhachHang != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try { await _emailService.GuiEmailHoanHang(yc, yc.DonHang.KhachHang.Email); }
                        catch (Exception ex) { _logger.LogError(ex, "Lỗi gửi email"); }
                    });
                }

                return Json(new { success = true, message = "Xử lý thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xử lý");
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }
    }
}