// Họ và tên: Phạm Thái Hoàng
// Mã sinh viên: 23203100039
// Nội dung thực hiện: Dashboard, thong ke

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Models.ViewModels;

namespace QuanLyCuaHangDienTu.Controllers
{
    [AdminAuthorize]
    public class ThongKeController : Controller
    {
        private readonly AppDbContext _context;
        public ThongKeController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TongLoaiSanPham = await _context.LoaiSanPhams.CountAsync(),
                TongSanPham = await _context.SanPhams.CountAsync(),
                TongKhachHang = await _context.KhachHangs.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThai == "ChoXacNhan"),
                DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThai == "DangGiao"),
                DonHoanThanh = await _context.DonHangs.CountAsync(d => d.TrangThai == "DaHoanThanh"),
                SanPhamDangKinhDoanh = await _context.SanPhams.CountAsync(s => s.TrangThai),
                DoanhThuThang = await _context.DonHangs
                    .Where(d => d.TrangThai == "DaHoanThanh" && d.NgayDat.Month == DateTime.Now.Month && d.NgayDat.Year == DateTime.Now.Year)
                    .SumAsync(d => (decimal?)d.TongTien) ?? 0
            };
            return View(vm);
        }

        public async Task<IActionResult> ThongKe()
        {
            ViewBag.SanPhamTheoLoai = await _context.LoaiSanPhams
                .Select(l => new { l.TenLoai, SoLuong = l.SanPhams.Count }).ToListAsync();

            ViewBag.DonHangTheoTrangThai = await _context.DonHangs
                .GroupBy(d => d.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() }).ToListAsync();

            ViewBag.SanPhamBanChay = await _context.ChiTietDonHangs
                .Where(c => c.DonHang!.TrangThai == "DaHoanThanh")
                .GroupBy(c => c.SanPham!.TenSanPham)
                .Select(g => new { TenSanPham = g.Key, TongBan = g.Sum(c => c.SoLuong) })
                .OrderByDescending(x => x.TongBan).Take(10).ToListAsync();

            ViewBag.DoanhThuThang = await _context.DonHangs
                .Where(d => d.TrangThai == "DaHoanThanh")
                .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
                .Select(g => new { Thang = g.Key.Month, Nam = g.Key.Year, DoanhThu = g.Sum(d => d.TongTien) })
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang).ToListAsync();

            return View();
        }
    }
}