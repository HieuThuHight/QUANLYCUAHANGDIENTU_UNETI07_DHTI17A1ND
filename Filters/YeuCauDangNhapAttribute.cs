using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Filters
{
    public class YeuCauDangNhapAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32("MaTaiKhoan");

            if (maTaiKhoan == null)
            {
                var returnUrl = context.HttpContext.Request.Path
                              + context.HttpContext.Request.QueryString;

                context.Result = new RedirectToActionResult(
                    "DangNhap", "TaiKhoan", new { returnUrl = returnUrl.ToString() });
            }
        }
    }

}
