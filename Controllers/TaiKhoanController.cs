using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _context;

        public TaiKhoanController(AppDbContext context)
        {
            _context = context;
        }

        // ===== GET: /TaiKhoan/DangNhap =====
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ===== POST: /TaiKhoan/DangNhap =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Truy vấn tài khoản bằng LINQ
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap
                                       && t.MatKhau == model.MatKhau);

            if (taiKhoan == null)
            {
                ViewBag.Loi = "Tên đăng nhập hoặc mật khẩu không đúng!";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ViewBag.Loi = "Tài khoản đã bị khóa. Vui lòng liên hệ Admin!";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Lưu Session: MaTaiKhoan, HoTen, VaiTro
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ===== GET: /TaiKhoan/DangXuat =====
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }
}
