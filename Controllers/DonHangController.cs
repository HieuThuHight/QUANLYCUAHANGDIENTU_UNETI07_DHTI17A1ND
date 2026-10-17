using Microsoft.AspNetCore.Mvc;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class DonHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
