// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Phan quyen Admin, KhachHang, BatBuocDangNhap

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