// ============================================================================
// HỌ VÀ TÊN: Vương Quang Đạt
// MÃ SINH VIÊN: 23203100036
// LỚP: DHTI17A1ND
// EMAIL: vqdat.dhti17a1nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 3 - GIỎ HÀNG, ĐẶT HÀNG, THANH TOÁN VIETQR/COD & KHÁCH HÀNG
// NỘI DUNG: Quản lý danh sách khách hàng, cập nhật hồ sơ và khóa/mở khóa tài khoản khách
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;
using QuanLyCuaHangDienTu.Models.ViewModels;

namespace QuanLyCuaHangDienTu.Controllers
{
    // Quản lý khách hàng - CHỈ ADMIN
    [AdminAuthorize]
    public class KhachHangController : Controller
    {
        private readonly AppDbContext _context;
        public KhachHangController(AppDbContext context) => _context = context;

        //public async Task<IActionResult> Index(string? keyword, int pageIndex = 1)
        //{
        //    var query = _context.KhachHangs.Include(k => k.TaiKhoan).AsNoTracking().AsQueryable();
        //    if (!string.IsNullOrWhiteSpace(keyword))
        //        query = query.Where(k => k.HoTen.Contains(keyword) || k.SoDienThoai.Contains(keyword));

        //    ViewBag.Keyword = keyword;
        //    var list = await PaginatedList<KhachHang>.CreateAsync(query.OrderBy(k => k.HoTen), pageIndex, 10);
        //    return View(list);
        //}
        public async Task<IActionResult> Index(KhachHangFilterViewModel filter, int pageIndex = 1)
        {
            var query = _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .AsNoTracking()
                .AsQueryable();

            // 1. Tìm kiếm gần đúng (Tên, SĐT, Email)
            if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
            {
                var keyword = filter.TuKhoa.Trim();
                query = query.Where(k =>
                    EF.Functions.Like(k.HoTen, $"%{keyword}%") ||
                    EF.Functions.Like(k.SoDienThoai, $"%{keyword}%") ||
                    (k.Email != null && EF.Functions.Like(k.Email, $"%{keyword}%"))
                );
            }

            // 2. Lọc theo địa chỉ (gần đúng)
            if (!string.IsNullOrWhiteSpace(filter.DiaChi))
            {
                var diaChi = filter.DiaChi.Trim();
                query = query.Where(k => k.DiaChi != null && EF.Functions.Like(k.DiaChi, $"%{diaChi}%"));
            }

            // 3. Lọc theo khoảng ngày đăng ký
            if (filter.TuNgayDangKy.HasValue)
                query = query.Where(k => k.NgayDangKy >= filter.TuNgayDangKy.Value.Date);

            if (filter.DenNgayDangKy.HasValue)
            {
                var denNgay = filter.DenNgayDangKy.Value.Date.AddDays(1);
                query = query.Where(k => k.NgayDangKy < denNgay);
            }

            var list = await PaginatedList<KhachHang>.CreateAsync(
                query.OrderBy(k => k.HoTen), pageIndex, 10);

            filter.DanhSachKhachHang = list; // nếu dùng PaginatedList, cần điều chỉnh
            ViewBag.Filter = filter;
            return View(list); // Truyền thẳng PaginatedList ra View
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
            var kh = await _context.KhachHangs.Include(k => k.TaiKhoan).FirstOrDefaultAsync(k => k.MaKhachHang == id);
            if (kh == null) return Json(new { success = false, message = "Không tìm thấy khách hàng." });
            
            kh.TrangThai = !kh.TrangThai;
            if (kh.TaiKhoan != null)
            {
                kh.TaiKhoan.TrangThai = kh.TrangThai; // Đồng bộ khóa cả tài khoản đăng nhập
            }
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
