// ============================================================================
// HỌ VÀ TÊN: Đỗ Văn Hiếu
// MÃ SINH VIÊN: 23203100018
// LỚP: DHTI17A1ND
// EMAIL: dvhieu.dhti17a1nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 1 - XÁC THỰC, PHÂN QUYỀN, TÀI KHOẢN & LOẠI SẢN PHẨM
// NỘI DUNG: Trang chủ, điều hướng theo vai trò và danh sách sản phẩm nổi bật
// ============================================================================

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
