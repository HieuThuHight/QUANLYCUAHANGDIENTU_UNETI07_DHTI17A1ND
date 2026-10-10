using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Filters
{
    public class YeuCauAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = context.HttpContext.Session.GetString("VaiTro");

            // Chưa đăng nhập → về Login
            if (maTaiKhoan == null)
            {
                var returnUrl = context.HttpContext.Request.Path
                              + context.HttpContext.Request.QueryString;

                context.Result = new RedirectToActionResult(
                    "DangNhap", "TaiKhoan", new { returnUrl = returnUrl.ToString() });
                return;
            }

            // Không phải Admin → về Home
            if (vaiTro != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
