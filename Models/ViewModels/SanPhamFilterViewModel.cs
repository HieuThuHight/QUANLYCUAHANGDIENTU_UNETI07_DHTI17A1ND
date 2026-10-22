// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Module 2: Danh sách/lọc sản phẩm, phân trang và hiển thị chi tiết sản phẩm.

using QuanLyCuaHangDienTu.Helpers;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class SanPhamFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? MaLoai { get; set; }
        public string? ThuongHieu { get; set; }
        public bool? TrangThai { get; set; }
        public decimal? GiaTu { get; set; }
        public decimal? GiaDen { get; set; }
        public string? SortOrder { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public PaginatedList<SanPham>? SanPhams { get; set; }
    }
}