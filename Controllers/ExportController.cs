// ============================================================================
// HỌ VÀ TÊN: Lưu Đức Huy
// MÃ SINH VIÊN: 23274800002
// LỚP: DHTI17A2ND
// EMAIL: ldhuy.dhti17a2nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 2 - SẢN PHẨM, VOUCHER, HOÀN HÀNG & BÁO CÁO EXCEL/PDF
// NỘI DUNG: Xuất dữ liệu danh sách sản phẩm và tồn kho ra tệp tin Excel ClosedXML
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Services;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class ExportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ExcelExportService _excelService;
        private readonly PdfExportService _pdfService;
        private readonly ILogger<ExportController> _logger;

        public ExportController(AppDbContext context, ExcelExportService excelService, PdfExportService pdfService, ILogger<ExportController> logger)
        {
            _context = context;
            _excelService = excelService;
            _pdfService = pdfService;
            _logger = logger;
        }

        // XUẤT EXCEL - CHỈ ADMIN
        [AdminAuthorize]
        public async Task<IActionResult> ExcelSanPham()
        {
            var list = await _context.SanPhams.Include(s => s.LoaiSanPham).AsNoTracking().ToListAsync();
            var bytes = _excelService.ExportSanPham(list);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"SanPham_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        // XUẤT PDF HÓA ĐƠN - CẢ ADMIN VÀ KHÁCH
        public async Task<IActionResult> PdfHoaDon(int id)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (maTaiKhoan == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem hóa đơn.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Url.Action("PdfHoaDon", new { id }) });
            }

            var dh = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs).ThenInclude(c => c.SanPham)
                .Include(d => d.Voucher)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (dh == null) return NotFound();

            if (vaiTro != "Admin")
            {
                var maKH = HttpContext.Session.GetInt32("MaKhachHang");
                if (maKH == null || dh.MaKhachHang != maKH.Value)
                {
                    TempData["Error"] = "Bạn không có quyền xem hóa đơn này.";
                    return RedirectToAction("Index", "DonHang");
                }
            }

            try
            {
                var bytes = _pdfService.XuatHoaDon(dh);
                return File(bytes, "application/pdf", $"HoaDon_{id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xuất PDF hóa đơn #{MaDonHang}", id);
                TempData["Error"] = "Có lỗi khi xuất hóa đơn.";
                return RedirectToAction("Index", "DonHang");
            }
        }
    }
}