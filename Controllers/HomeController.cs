using Microsoft.AspNetCore.Mvc;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
using System.Diagnostics;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
