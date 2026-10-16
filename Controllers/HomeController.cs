// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Trang chu - redirect theo vai tro

using Microsoft.AspNetCore.Mvc;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro == "Admin")
                return RedirectToAction("Index", "ThongKe");

            return View();
        }

        public IActionResult Error() => View();
    }
}