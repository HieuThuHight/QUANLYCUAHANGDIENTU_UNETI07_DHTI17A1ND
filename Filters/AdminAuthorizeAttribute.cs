// ============================================================================
// HỌ VÀ TÊN: Đỗ Văn Hiếu
// MÃ SINH VIÊN: 23203100018
// LỚP: DHTI17A1ND
// EMAIL: dvhieu.dhti17a1nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 1 - XÁC THỰC, PHÂN QUYỀN, TÀI KHOẢN & LOẠI SẢN PHẨM
// NỘI DUNG: ActionFilter kiểm tra phân quyền truy cập Admin, KhachHang và bắt buộc đăng nhập
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyCuaHangDienTu.Filters
{
    // Chỉ Admin
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTro = context.HttpContext.Session.GetString("VaiTro");
            if (string.IsNullOrEmpty(vaiTro) || vaiTro != "Admin")
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan",
                    new { returnUrl = returnUrl.ToString() });
            }
            base.OnActionExecuting(context);
        }
    }

    // Chỉ Khách hàng (Admin bị chặn)
    public class KhachHangAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTro = context.HttpContext.Session.GetString("VaiTro");
            var maTaiKhoan = context.HttpContext.Session.GetInt32("MaTaiKhoan");

            if (maTaiKhoan == null || vaiTro == "Admin")
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan",
                    new { returnUrl = returnUrl.ToString() });
            }
            base.OnActionExecuting(context);
        }
    }

    // Bắt buộc đăng nhập (cả Admin và Khách)
    public class BatBuocDangNhapAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan",
                    new { returnUrl = returnUrl.ToString() });
            }
            base.OnActionExecuting(context);
        }
    }
}