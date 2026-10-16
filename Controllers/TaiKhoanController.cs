// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Dang nhap, Dang ky, Dang xuat, Quen mat khau

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;
using QuanLyCuaHangDienTu.Models.ViewModels;
using QuanLyCuaHangDienTu.Services;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(AppDbContext context, EmailService emailService, ILogger<TaiKhoanController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // ĐĂNG NHẬP
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                var vaiTro = HttpContext.Session.GetString("VaiTro");
                return vaiTro == "Admin"
                    ? RedirectToAction("Index", "ThongKe")
                    : RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var tk = await _context.TaiKhoans
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

            if (tk == null || !PasswordHelper.VerifyPassword(model.MatKhau, tk.MatKhau))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (!tk.TrangThai)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa.");
                return View(model);
            }

            HttpContext.Session.SetInt32("MaTaiKhoan", tk.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", tk.HoTen);
            HttpContext.Session.SetString("VaiTro", tk.VaiTro);

            if (tk.VaiTro == "KhachHang")
            {
                var kh = await _context.KhachHangs.AsNoTracking()
                    .FirstOrDefaultAsync(k => k.MaTaiKhoan == tk.MaTaiKhoan);
                if (kh != null)
                    HttpContext.Session.SetInt32("MaKhachHang", kh.MaKhachHang);
            }

            _logger.LogInformation("User {TenDangNhap} đăng nhập", tk.TenDangNhap);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return tk.VaiTro == "Admin"
                ? RedirectToAction("Index", "ThongKe")
                : RedirectToAction("Index", "Home");
        }

        // ĐĂNG KÝ
        [HttpGet]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }
            if (await _context.TaiKhoans.AnyAsync(t => t.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }
            if (await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == model.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng.");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tk = new TaiKhoan
                {
                    TenDangNhap = model.TenDangNhap,
                    MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                    HoTen = model.HoTen,
                    Email = model.Email,
                    VaiTro = "KhachHang",
                    TrangThai = true
                };
                _context.TaiKhoans.Add(tk);
                await _context.SaveChangesAsync();

                var kh = new KhachHang
                {
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    DiaChi = model.DiaChi,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };
                _context.KhachHangs.Add(kh);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi đăng ký");
                ModelState.AddModelError("", "Có lỗi xảy ra.");
                return View(model);
            }
        }

        // ĐĂNG XUẤT
        public IActionResult DangXuat()
        {
            var hoTen = HttpContext.Session.GetString("HoTen");
            HttpContext.Session.Clear();
            _logger.LogInformation("User {HoTen} đăng xuất", hoTen);
            TempData["Success"] = "Đã đăng xuất.";
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        // KIỂM TRA USERNAME (AJAX)
        [HttpGet]
        public async Task<IActionResult> KiemTraTenDangNhap(string tenDangNhap)
        {
            var exists = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap);
            return Json(new { available = !exists });
        }
    }
}