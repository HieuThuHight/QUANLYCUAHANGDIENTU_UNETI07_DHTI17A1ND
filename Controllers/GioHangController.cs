// Họ và tên: Vương Quang Đạt
// Mã sinh viên: 23203100036
// Nội dung thực hiện: Gio hang, dat hang - bat buoc dang nhap

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Models.Entities;
using QuanLyCuaHangDienTu.Models.ViewModels;
using QuanLyCuaHangDienTu.Services;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class GioHangController : Controller
    {
        private const string CART_KEY = "GIOHANG";
        private readonly AppDbContext _context;
        private readonly VoucherService _voucherService;
        private readonly EmailService _emailService;
        private readonly ILogger<GioHangController> _logger;

        public GioHangController(AppDbContext context, VoucherService voucherService, EmailService emailService, ILogger<GioHangController> logger)
        {
            _context = context;
            _voucherService = voucherService;
            _emailService = emailService;
            _logger = logger;
        }

        private List<CartItemViewModel> GetCart()
        {
            var json = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(json) ? new() : JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new();
        }

        private void SaveCart(List<CartItemViewModel> cart)
            => HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(cart));

        // ===== THÊM VÀO GIỎ - CHỈ KHÁCH ĐÃ ĐĂNG NHẬP =====
        [HttpPost]
        [KhachHangAuthorize]
        public async Task<IActionResult> Them([FromBody] CartItemViewModel item)
        {
            if (item == null || item.SoLuong <= 0)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            var sp = await _context.SanPhams.FindAsync(item.MaSanPham);
            if (sp == null || !sp.TrangThai)
                return Json(new { success = false, message = "Sản phẩm không khả dụng." });

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.MaSanPham == item.MaSanPham);

            if (existing != null)
            {
                if (existing.SoLuong + item.SoLuong > sp.SoLuongTon)
                    return Json(new { success = false, message = $"Chỉ còn {sp.SoLuongTon}." });
                existing.SoLuong += item.SoLuong;
            }
            else
            {
                if (item.SoLuong > sp.SoLuongTon)
                    return Json(new { success = false, message = $"Chỉ còn {sp.SoLuongTon}." });
                cart.Add(new CartItemViewModel
                {
                    MaSanPham = sp.MaSanPham,
                    TenSanPham = sp.TenSanPham,
                    DonGia = sp.DonGia,
                    SoLuong = item.SoLuong,
                    HinhAnh = sp.HinhAnh
                });
            }
            SaveCart(cart);
            return Json(new { success = true, totalItems = cart.Sum(c => c.SoLuong) });
        }

        // ===== XEM GIỎ - CHỈ KHÁCH =====
        [KhachHangAuthorize]
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.TongTien = cart.Sum(c => c.ThanhTien);
            return View(cart);
        }

        // ===== CẬP NHẬT - CHỈ KHÁCH =====
        [HttpPost]
        [KhachHangAuthorize]
        public async Task<IActionResult> CapNhat(int maSanPham, int soLuong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MaSanPham == maSanPham);
            if (item == null) return Json(new { success = false });

            if (soLuong <= 0) cart.Remove(item);
            else
            {
                var sp = await _context.SanPhams.FindAsync(maSanPham);
                if (sp == null || soLuong > sp.SoLuongTon)
                    return Json(new { success = false, message = $"Chỉ còn {sp?.SoLuongTon ?? 0}." });
                item.SoLuong = soLuong;
            }
            SaveCart(cart);
            return Json(new { success = true, tongTien = cart.Sum(c => c.ThanhTien) });
        }

        // ===== XÓA - CHỈ KHÁCH =====
        [HttpPost]
        [KhachHangAuthorize]
        public IActionResult Xoa(int maSanPham)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.MaSanPham == maSanPham);
            SaveCart(cart);
            return Json(new { success = true, totalItems = cart.Sum(c => c.SoLuong) });
        }

        // ===== THANH TOÁN - CHỈ KHÁCH =====
        [KhachHangAuthorize]
        public async Task<IActionResult> ThanhToan()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "SanPham");

            var maKH = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var kh = await _context.KhachHangs.FindAsync(maKH.Value);

            // Kiểm tra số lượng tồn và trạng thái sản phẩm trong giỏ trước khi vào trang thanh toán
            bool cartModified = false;
            var listXoa = new List<CartItemViewModel>();
            var warnings = new List<string>();

            foreach (var item in cart)
            {
                var sp = await _context.SanPhams.FindAsync(item.MaSanPham);
                if (sp == null || !sp.TrangThai || sp.SoLuongTon <= 0)
                {
                    listXoa.Add(item);
                    warnings.Add($"Sản phẩm '{item.TenSanPham}' đã hết hàng hoặc ngừng kinh doanh và đã được xóa khỏi giỏ.");
                    cartModified = true;
                }
                else if (item.SoLuong > sp.SoLuongTon)
                {
                    warnings.Add($"Sản phẩm '{item.TenSanPham}' chỉ còn {sp.SoLuongTon} sản phẩm trong kho. Đã tự động cập nhật số lượng.");
                    item.SoLuong = sp.SoLuongTon;
                    cartModified = true;
                }
            }

            foreach (var x in listXoa)
            {
                cart.Remove(x);
            }

            if (cartModified)
            {
                SaveCart(cart);
                if (warnings.Any())
                {
                    TempData["Warning"] = string.Join(" ", warnings);
                }
                if (!cart.Any())
                {
                    TempData["Error"] = "Tất cả sản phẩm trong giỏ hàng hiện đã hết hàng hoặc không khả dụng.";
                    return RedirectToAction("Index", "GioHang");
                }
            }

            return View(new CheckoutViewModel
            {
                Items = cart,
                DiaChiGiaoHang = kh?.DiaChi ?? "",
                Vouchers = await _context.Vouchers
                    .Where(v => v.TrangThai && v.NgayKetThuc.Date >= DateTime.Now.Date && v.SoLuong > v.DaSuDung)
                    .OrderBy(v => v.NgayKetThuc).ToListAsync()
            });
        }

        // ===== ĐẶT HÀNG =====
        [HttpPost]
        [KhachHangAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "SanPham");

            var maKHNullable = HttpContext.Session.GetInt32("MaKhachHang");
            if (maKHNullable == null)
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            var maKH = maKHNullable.Value;

            // Kiểm tra khách hàng tồn tại và tài khoản đang hoạt động
            var khachHang = await _context.KhachHangs.Include(k => k.TaiKhoan).FirstOrDefaultAsync(k => k.MaKhachHang == maKH);
            if (khachHang == null || !khachHang.TrangThai || khachHang.TaiKhoan == null || !khachHang.TaiKhoan.TrangThai)
            {
                ModelState.AddModelError("", "Tài khoản khách hàng không tồn tại hoặc đã bị khóa.");
                return await ReloadCheckoutView(model, cart);
            }

            if (string.IsNullOrWhiteSpace(model.DiaChiGiaoHang))
            {
                ModelState.AddModelError("DiaChiGiaoHang", "Địa chỉ giao hàng không được để trống.");
                return await ReloadCheckoutView(model, cart);
            }

            // Sử dụng Serializable Transaction để đảm bảo tính toàn vẹn kho hàng và chống race condition (đặt hàng đồng thời)
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                // Kiểm tra lại toàn bộ giỏ hàng với dữ liệu tồn kho mới nhất trong DB
                foreach (var item in cart)
                {
                    var sp = await _context.SanPhams.FindAsync(item.MaSanPham);
                    if (sp == null || !sp.TrangThai)
                    {
                        ModelState.AddModelError("", $"Sản phẩm '{item.TenSanPham}' không còn khả dụng trên hệ thống.");
                        await transaction.RollbackAsync();
                        return await ReloadCheckoutView(model, cart);
                    }
                    if (item.SoLuong <= 0)
                    {
                        ModelState.AddModelError("", $"Số lượng đặt của '{item.TenSanPham}' phải lớn hơn 0.");
                        await transaction.RollbackAsync();
                        return await ReloadCheckoutView(model, cart);
                    }
                    if (sp.SoLuongTon <= 0)
                    {
                        ModelState.AddModelError("", $"Sản phẩm '{item.TenSanPham}' hiện đã hết hàng trong kho.");
                        await transaction.RollbackAsync();
                        return await ReloadCheckoutView(model, cart);
                    }
                    if (item.SoLuong > sp.SoLuongTon)
                    {
                        ModelState.AddModelError("", $"Sản phẩm '{item.TenSanPham}' chỉ còn lại {sp.SoLuongTon} sản phẩm trong kho (bạn đang đặt {item.SoLuong}).");
                        await transaction.RollbackAsync();
                        return await ReloadCheckoutView(model, cart);
                    }
                    // Đồng bộ lại đơn giá chính xác từ database
                    item.DonGia = sp.DonGia;
                }

                var tongTien = cart.Sum(c => c.ThanhTien);
                decimal giamGia = 0;
                int? maVoucher = null;

                if (!string.IsNullOrEmpty(model.MaCodeVoucher))
                {
                    var (ok, giam, msg) = await _voucherService.ApDungVoucher(model.MaCodeVoucher, tongTien);
                    if (!ok)
                    {
                        ModelState.AddModelError("", msg);
                        await transaction.RollbackAsync();
                        return await ReloadCheckoutView(model, cart);
                    }
                    giamGia = giam;
                    var v = await _context.Vouchers.FirstOrDefaultAsync(x => x.MaCode == model.MaCodeVoucher);
                    if (v == null || v.DaSuDung >= v.SoLuong)
                    {
                        ModelState.AddModelError("", "Mã voucher này đã hết lượt sử dụng.");
                        await transaction.RollbackAsync();
                        return await ReloadCheckoutView(model, cart);
                    }
                    maVoucher = v.MaVoucher;
                    v.DaSuDung++;
                }

                // Trừ số lượng tồn kho sản phẩm ngay khi đặt hàng thành công
                foreach (var item in cart)
                {
                    var sp = await _context.SanPhams.FindAsync(item.MaSanPham);
                    sp!.SoLuongTon -= item.SoLuong;
                }

                var phuongThuc = model.PhuongThucThanhToan == "COD" ? "COD" : "VietQR";
                var ghiChuDayDu = $"[PTTT: {phuongThuc}]" + (string.IsNullOrWhiteSpace(model.GhiChu) ? "" : $" | {model.GhiChu.Trim()}");

                var donHang = new DonHang
                {
                    MaKhachHang = maKH,
                    NgayDat = DateTime.Now,
                    DiaChiGiaoHang = model.DiaChiGiaoHang,
                    GhiChu = ghiChuDayDu,
                    TongTien = tongTien - giamGia,
                    GiamGia = giamGia,
                    MaVoucher = maVoucher,
                    TrangThai = "ChoXacNhan",
                    ChiTietDonHangs = cart.Select(c => new ChiTietDonHang
                    {
                        MaSanPham = c.MaSanPham,
                        SoLuong = c.SoLuong,
                        DonGia = c.DonGia,
                        ThanhTien = c.ThanhTien
                    }).ToList()
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _context.Entry(donHang).Reference(d => d.KhachHang).LoadAsync();
                if (donHang.KhachHang != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try { await _emailService.GuiEmailDatHangThanhCong(donHang, donHang.KhachHang.Email); }
                        catch (Exception ex) { _logger.LogError(ex, "Lỗi gửi email"); }
                    });
                }

                HttpContext.Session.Remove(CART_KEY);
                if (phuongThuc == "COD")
                {
                    TempData["Success"] = $"Đặt hàng thành công! Mã đơn: #{donHang.MaDonHang}. Phương thức: Tiền mặt khi nhận hàng (COD).";
                }
                else
                {
                    TempData["Success"] = $"Đặt hàng thành công! Vui lòng quét mã VietQR bên dưới để thanh toán đơn hàng #{donHang.MaDonHang}.";
                }
                return RedirectToAction("Details", "DonHang", new { id = donHang.MaDonHang });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình đặt hàng");
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại!");
                return await ReloadCheckoutView(model, cart);
            }
        }

        private async Task<IActionResult> ReloadCheckoutView(CheckoutViewModel model, List<CartItemViewModel> cart)
        {
            model.Items = cart;
            model.Vouchers = await _context.Vouchers
                .Where(v => v.TrangThai && v.NgayKetThuc.Date >= DateTime.Now.Date && v.SoLuong > v.DaSuDung)
                .ToListAsync();
            return View("ThanhToan", model);
        }

        // ===== KIỂM TRA VOUCHER (AJAX) =====
        [HttpPost]
        [KhachHangAuthorize]
        public async Task<IActionResult> KiemTraVoucher([FromBody] VoucherCheckRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.MaCode))
                return Json(new { success = false, giam = 0, message = "Vui lòng nhập mã." });

            var (ok, giam, msg) = await _voucherService.ApDungVoucher(req.MaCode, req.TongTien);
            return Json(new { success = ok, giam = ok ? giam : 0, message = msg });
        }

        // ===== ĐẾM SỐ LƯỢNG GIỎ =====
        [HttpGet]
        public IActionResult SoLuong()
        {
            var cart = GetCart();
            return Json(new { totalItems = cart.Sum(c => c.SoLuong) });
        }
    }

    public class VoucherCheckRequest
    {
        public string MaCode { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
    }
}
