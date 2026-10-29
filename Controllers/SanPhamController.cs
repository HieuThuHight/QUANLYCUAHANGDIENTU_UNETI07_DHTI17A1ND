// ============================================================================
// HỌ VÀ TÊN: Lưu Đức Huy
// MÃ SINH VIÊN: 23274800002
// LỚP: DHTI17A2ND
// EMAIL: ldhuy.dhti17a2nd@sv.uneti.edu.vn
// PHÂN HỆ: MODULE 2 - SẢN PHẨM, VOUCHER, HOÀN HÀNG & BÁO CÁO EXCEL/PDF
// NỘI DUNG: CRUD Sản phẩm, bộ lọc đa tiêu chí, upload ảnh đa thư mục wwwroot, báo lỗi trùng tên và đơn giá <= 0
// ============================================================================

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
            ModelState.Remove("LoaiSanPham");
            ModelState.Remove("ChiTietDonHangs");
            if (fileAnh != null && fileAnh.Length > 0) { ModelState.Remove("HinhAnh"); }
            // 1. Chuẩn hóa & kiểm tra trùng tên sản phẩm (không phân biệt hoa thường)
            sp.TenSanPham = sp.TenSanPham?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(sp.TenSanPham) &&
                await _context.SanPhams.AnyAsync(s => s.TenSanPham.ToLower() == sp.TenSanPham.ToLower()))
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm này đã tồn tại trong hệ thống. Vui lòng nhập tên khác.");
            }

            // 2. Báo lỗi khi đơn giá <= 0
            if (sp.DonGia <= 0)
            {
                ModelState.AddModelError("DonGia", "Đơn giá sản phẩm phải lớn hơn 0 VNĐ.");
            }

            // 3. Kiểm tra số lượng tồn không được âm
            if (sp.SoLuongTon < 0)
            {
                ModelState.AddModelError("SoLuongTon", "Số lượng tồn kho không được nhỏ hơn 0.");
            }

            // 4. Xử lý ảnh tải lên
            if (fileAnh != null && fileAnh.Length > 0)
            {
                try 
                { 
                    sp.HinhAnh = await SaveImage(fileAnh); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi upload ảnh");
                    ModelState.AddModelError("HinhAnh", "Lỗi tải ảnh: " + ex.Message);
                }
            }
            else if (string.IsNullOrWhiteSpace(sp.HinhAnh))
            {
                sp.HinhAnh = "/images/no-image.png";
            }

            if (!ModelState.IsValid) 
            { 
                await LoadDropdowns(sp.MaLoai); 
                return View(sp); 
            }

            try
            {
                _context.SanPhams.Add(sp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm sản phẩm mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi thêm sản phẩm");
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu sản phẩm: " + ex.Message);
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
            ModelState.Remove("LoaiSanPham");
            ModelState.Remove("ChiTietDonHangs");
            if (fileAnh != null && fileAnh.Length > 0) { ModelState.Remove("HinhAnh"); }
            if (id != sp.MaSanPham) return NotFound();

            // 1. Chuẩn hóa & kiểm tra trùng tên sản phẩm với sản phẩm khác (không phân biệt hoa thường)
            sp.TenSanPham = sp.TenSanPham?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(sp.TenSanPham) &&
                await _context.SanPhams.AnyAsync(s => s.TenSanPham.ToLower() == sp.TenSanPham.ToLower() && s.MaSanPham != id))
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm này đã được sử dụng bởi sản phẩm khác. Vui lòng chọn tên khác.");
            }

            // 2. Báo lỗi khi đơn giá <= 0
            if (sp.DonGia <= 0)
            {
                ModelState.AddModelError("DonGia", "Đơn giá sản phẩm phải lớn hơn 0 VNĐ.");
            }

            // 3. Kiểm tra số lượng tồn không được âm
            if (sp.SoLuongTon < 0)
            {
                ModelState.AddModelError("SoLuongTon", "Số lượng tồn kho không được nhỏ hơn 0.");
            }

            // 4. Xử lý ảnh cập nhật
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
                    ModelState.AddModelError("HinhAnh", "Lỗi tải ảnh: " + ex.Message);
                }
            }
            else if (string.IsNullOrWhiteSpace(sp.HinhAnh))
            {
                sp.HinhAnh = "/images/no-image.png";
            }

            if (!ModelState.IsValid) 
            { 
                await LoadDropdowns(sp.MaLoai); 
                return View(sp); 
            }

            try
            {
                _context.SanPhams.Update(sp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật sản phẩm #{MaSanPham}", id);
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
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
            if (sp == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            if (sp.ChiTietDonHangs.Any())
                return Json(new { success = false, message = "Không thể xóa vì sản phẩm đã có trong đơn hàng." });

            try
            {
                DeleteOldImage(sp.HinhAnh);
                _context.SanPhams.Remove(sp);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa sản phẩm #{MaSanPham}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
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
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".jfif", ".bmp", ".avif", ".svg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                throw new Exception("Chỉ chấp nhận các định dạng ảnh: JPG, JPEG, PNG, GIF, WEBP, JFIF, BMP, AVIF, SVG.");

            if (file.Length > 15 * 1024 * 1024)
                throw new Exception("Dung lượng tập tin ảnh không được vượt quá 15MB.");

            var fileName = Guid.NewGuid().ToString("N") + ext;

            // Đọc toàn bộ nội dung file ảnh một lần vào bộ nhớ để ghi an toàn cho nhiều thư mục
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Tìm tất cả các thư mục wwwroot có thể có để lưu đồng bộ
            var targetFolders = new List<string>();

            if (!string.IsNullOrEmpty(_env.WebRootPath))
            {
                targetFolders.Add(Path.Combine(_env.WebRootPath, "images", "products"));
            }

            if (!string.IsNullOrEmpty(_env.ContentRootPath))
            {
                targetFolders.Add(Path.Combine(_env.ContentRootPath, "wwwroot", "images", "products"));
            }

            var baseFolder = Path.Combine(AppContext.BaseDirectory, "wwwroot", "images", "products");
            targetFolders.Add(baseFolder);

            try
            {
                var projFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot", "images", "products"));
                targetFolders.Add(projFolder);
            }
            catch { }

            bool savedAny = false;
            foreach (var folder in targetFolders.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    var filePath = Path.Combine(folder, fileName);
                    await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
                    savedAny = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Không thể lưu ảnh vào thư mục: {Folder}", folder);
                }
            }

            if (!savedAny)
            {
                throw new Exception("Không thể lưu tập tin ảnh vào máy chủ. Vui lòng kiểm tra quyền ghi thư mục.");
            }

            return "/images/products/" + fileName;
        }

        private void DeleteOldImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            if (!imagePath.StartsWith("/images/products/")) return;

            var fileName = Path.GetFileName(imagePath);
            var targets = new List<string>();
            if (!string.IsNullOrEmpty(_env.WebRootPath)) targets.Add(Path.Combine(_env.WebRootPath, "images", "products", fileName));
            if (!string.IsNullOrEmpty(_env.ContentRootPath)) targets.Add(Path.Combine(_env.ContentRootPath, "wwwroot", "images", "products", fileName));
            targets.Add(Path.Combine(AppContext.BaseDirectory, "wwwroot", "images", "products", fileName));
            targets.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot", "images", "products", fileName)));

            foreach (var fullPath in targets.Distinct())
            {
                try
                {
                    if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                }
                catch { /* ignore */ }
            }
        }
    }
}
