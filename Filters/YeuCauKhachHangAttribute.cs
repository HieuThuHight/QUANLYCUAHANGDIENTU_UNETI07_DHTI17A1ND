using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Filters
{
    public class YeuCauKhachHangAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = context.HttpContext.Session.GetString("VaiTro");

            if (maTaiKhoan == null)
            {
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
                return;
            }

            if (vaiTro != "KhachHang")
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }

}
