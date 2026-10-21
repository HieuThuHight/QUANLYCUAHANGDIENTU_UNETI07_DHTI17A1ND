// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Quan ly san pham, tim kiem, loc, sap xep, phan trang, upload anh

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Filters;
using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;
using QuanLyCuaHangDienTu.Models.ViewModels;

namespace QuanLyCuaHangDienTu.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SanPhamController> _logger;

        public SanPhamController(AppDbContext context, IWebHostEnvironment env, ILogger<SanPhamController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // 1. DANH SÁCH (Tìm kiếm, Lọc, Sắp xếp, Phân trang)
        public async Task<IActionResult> Index(SanPhamFilterViewModel filter)
        {
            filter ??= new SanPhamFilterViewModel();
            var query = _context.SanPhams.Include(s => s.LoaiSanPham).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(s => s.TenSanPham.Contains(kw) || s.ThuongHieu.Contains(kw));
            }
            if (filter.MaLoai.HasValue) query = query.Where(s => s.MaLoai == filter.MaLoai.Value);
            if (!string.IsNullOrWhiteSpace(filter.ThuongHieu)) query = query.Where(s => s.ThuongHieu == filter.ThuongHieu);
            if (filter.TrangThai.HasValue) query = query.Where(s => s.TrangThai == filter.TrangThai.Value);
            if (filter.GiaTu.HasValue) query = query.Where(s => s.DonGia >= filter.GiaTu.Value);
            if (filter.GiaDen.HasValue) query = query.Where(s => s.DonGia <= filter.GiaDen.Value);

            query = filter.SortOrder switch
            {
                "ten_az" => query.OrderBy(s => s.TenSanPham),
                "ten_za" => query.OrderByDescending(s => s.TenSanPham),
                "gia_tang" => query.OrderBy(s => s.DonGia),
                "gia_giam" => query.OrderByDescending(s => s.DonGia),
                "ton_tang" => query.OrderBy(s => s.SoLuongTon),
                "ton_giam" => query.OrderByDescending(s => s.SoLuongTon),
                _ => query.OrderByDescending(s => s.MaSanPham)
            };

            filter.SanPhams = await PaginatedList<SanPham>.CreateAsync(query, filter.PageIndex, filter.PageSize);

            var loaiList = await _context.LoaiSanPhams.AsNoTracking()
                .Where(l => l.TrangThai).OrderBy(l => l.TenLoai).ToListAsync();
            ViewBag.LoaiSanPhams = new SelectList(loaiList, "MaLoai", "TenLoai", filter.MaLoai);
            ViewBag.ThuongHieus = await _context.SanPhams.AsNoTracking()
                .Where(s => s.TrangThai).Select(s => s.ThuongHieu).Distinct().OrderBy(s => s).ToListAsync();

            return View(filter);
        }

        // 2. CHI TIẾT
        public async Task<IActionResult> Details(int id)
        {
            var sp = await _context.SanPhams.Include(s => s.LoaiSanPham).AsNoTracking()
                .FirstOrDefaultAsync(s => s.MaSanPham == id);
            if (sp == null) return NotFound();

            ViewBag.SanPhamLienQuan = await _context.SanPhams.Include(s => s.LoaiSanPham).AsNoTracking()
                .Where(s => s.MaLoai == sp.MaLoai && s.MaSanPham != id && s.TrangThai)
                .Take(4).ToListAsync();

            return View(sp);
        }

        // 3. THÊM MỚI (Chỉ Admin)
        [AdminAuthorize]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> Create(SanPham sp, IFormFile? fileAnh)
        {
            if (fileAnh != null && fileAnh.Length > 0)
            {
                try { sp.HinhAnh = await SaveImage(fileAnh); }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi upload ảnh");
                    ModelState.AddModelError("HinhAnh", "Không thể upload ảnh: " + ex.Message);
                }
            }

            if (!ModelState.IsValid) { await LoadDropdowns(sp.MaLoai); return View(sp); }

            try
            {
                _context.SanPhams.Add(sp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thêm sản phẩm");
                ModelState.AddModelError("", "Có lỗi xảy ra.");
                await LoadDropdowns(sp.MaLoai);
                return View(sp);
            }
        }

        // 4. SỬA (Chỉ Admin)
        [AdminAuthorize]
        public async Task<IActionResult> Edit(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();
            await LoadDropdowns(sp.MaLoai);
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> Edit(int id, SanPham sp, IFormFile? fileAnh)
        {
            if (id != sp.MaSanPham) return NotFound();

            if (fileAnh != null && fileAnh.Length > 0)
            {
                try
                {
                    var oldImage = sp.HinhAnh;
                    sp.HinhAnh = await SaveImage(fileAnh);
                    DeleteOldImage(oldImage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi upload ảnh");
                    ModelState.AddModelError("HinhAnh", "Không thể upload ảnh: " + ex.Message);
                }
            }

            if (!ModelState.IsValid) { await LoadDropdowns(sp.MaLoai); return View(sp); }

            try
            {
                _context.SanPhams.Update(sp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật sản phẩm #{MaSanPham}", id);
                ModelState.AddModelError("", "Có lỗi xảy ra.");
                await LoadDropdowns(sp.MaLoai);
                return View(sp);
            }
        }

        // 5. XÓA (Chỉ Admin)
        [HttpPost]
        [AdminAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sp = await _context.SanPhams.Include(s => s.ChiTietDonHangs).FirstOrDefaultAsync(s => s.MaSanPham == id);
            if (sp == null) return Json(new { success = false, message = "Không tìm thấy." });
            if (sp.ChiTietDonHangs.Any())
                return Json(new { success = false, message = "Không thể xóa vì đã có trong đơn hàng." });

            try
            {
                DeleteOldImage(sp.HinhAnh);
                _context.SanPhams.Remove(sp);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa sản phẩm #{MaSanPham}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }

        // Helpers
        private async Task LoadDropdowns(int? selected = null)
        {
            var loaiList = await _context.LoaiSanPhams.AsNoTracking()
                .Where(l => l.TrangThai).OrderBy(l => l.TenLoai).ToListAsync();
            ViewBag.LoaiSanPhams = new SelectList(loaiList, "MaLoai", "TenLoai", selected);
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new Exception("Chỉ chấp nhận JPG, PNG, GIF, WEBP.");
            if (file.Length > 5 * 1024 * 1024) throw new Exception("File không quá 5MB.");

            var folder = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid().ToString("N") + ext;
            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);

            return "/images/products/" + fileName;
        }

        private void DeleteOldImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            if (!imagePath.StartsWith("/images/products/")) return;
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Không xóa được ảnh cũ: {Path}", imagePath); }
        }
    }
}