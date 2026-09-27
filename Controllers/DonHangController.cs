using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlycuahangdientu_uneti07_dhti17a1nd.Data;
using quanlycuahangdientu_uneti07_dhti17a1nd.Models;
using quanlycuahangdientu_uneti07_dhti17a1nd.ViewModels;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DatHang(int id)
        {
            var maKhachHang = LayMaKhachHangDangDangNhap();
            if (maKhachHang == null) return RedirectToAction("DangNhap", "TaiKhoan");
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();
            if (sanPham.TrangThai != "Đang kinh doanh")
            {
                TempData["Loi"] = "Sản phẩm hiện không kinh doanh, không thể đặt hàng.";
                return RedirectToAction("Index", "SanPham");
            }
            var viewModel = new DatHangViewModel
            {
                MaSanPham = sanPham.MaSanPham,
                TenSanPham = sanPham.TenSanPham,
                DonGia = sanPham.DonGia,
                SoLuongTon = sanPham.SoLuongTon,
                SoLuong = 1
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(DatHangViewModel model)
        {
            var maKhachHang = LayMaKhachHangDangDangNhap();
            if (maKhachHang == null) return RedirectToAction("DangNhap", "TaiKhoan");
            var khachHang = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .FirstOrDefaultAsync(k => k.MaKhachHang == maKhachHang.Value);
            var sanPham = await _context.SanPhams.FindAsync(model.MaSanPham);
            if (khachHang == null || !khachHang.TrangThai || khachHang.TaiKhoan == null || !khachHang.TaiKhoan.TrangThai)
            {
                TempData["Loi"] = "Tài khoản của bạn không thể thực hiện đặt hàng.";
                return RedirectToAction("Index", "SanPham");
            }
            if (sanPham == null)
            {
                ModelState.AddModelError(string.Empty, "Sản phẩm không tồn tại.");
            }
            else
            {
                if (sanPham.TrangThai != "Đang kinh doanh")
                    ModelState.AddModelError(string.Empty, "Sản phẩm hiện không kinh doanh.");
                if (model.SoLuong <= 0)
                    ModelState.AddModelError(nameof(model.SoLuong), "Số lượng đặt phải lớn hơn 0.");
                else if (model.SoLuong > sanPham.SoLuongTon)
                    ModelState.AddModelError(nameof(model.SoLuong), "Số lượng đặt vượt quá số lượng tồn.");
            }
            if (string.IsNullOrWhiteSpace(model.DiaChiGiaoHang))
                ModelState.AddModelError(nameof(model.DiaChiGiaoHang), "Địa chỉ giao hàng không được để trống.");
            if (!ModelState.IsValid)
            {
                if (sanPham != null)
                {
                    model.TenSanPham = sanPham.TenSanPham;
                    model.DonGia = sanPham.DonGia;
                    model.SoLuongTon = sanPham.SoLuongTon;
                }
                return View(model);
            }
            var donGiaTaiThoiDiem = sanPham!.DonGia;
            var thanhTien = donGiaTaiThoiDiem * model.SoLuong;
            var donHang = new DonHang
            {
                MaKhachHang = khachHang.MaKhachHang,
                NgayDat = DateTime.Now,
                DiaChiGiaoHang = model.DiaChiGiaoHang,
                TrangThai = "Chờ xác nhận",
                TongTien = thanhTien
            };
            donHang.ChiTietDonHangs.Add(new ChiTietDonHang
            {
                MaSanPham = sanPham.MaSanPham,
                SoLuong = model.SoLuong,
                DonGia = donGiaTaiThoiDiem,
                ThanhTien = thanhTien
            });
            sanPham.SoLuongTon -= model.SoLuong;
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = "Đặt hàng thành công.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Index(string? trangThai)
        {
            var maKhachHang = LayMaKhachHangDangDangNhap();
            if (maKhachHang == null) return RedirectToAction("DangNhap", "TaiKhoan");
            var truyVan = _context.DonHangs
                .Where(d => d.MaKhachHang == maKhachHang.Value)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(trangThai))
                truyVan = truyVan.Where(d => d.TrangThai == trangThai);
            ViewBag.TrangThaiHienTai = trangThai;
            var danhSach = await truyVan
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
            return View(danhSach);
        }
        public async Task<IActionResult> Details(int id)
        {
            var maKhachHang = LayMaKhachHangDangDangNhap();
            if (maKhachHang == null) return RedirectToAction("DangNhap", "TaiKhoan");
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.MaDonHang == id);
            if (donHang == null) return NotFound();
            if (donHang.MaKhachHang != maKhachHang.Value) return Forbid();
            return View(donHang);
        }
        private int? LayMaKhachHangDangDangNhap()
        {
            return HttpContext.Session.GetInt32("MaKhachHang");
        }
    }
}
