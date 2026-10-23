using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class KhachHangFilterViewModel
    {
        public string? TuKhoa { get; set; }          // Tìm Tên, SĐT, Email
        public string? DiaChi { get; set; }           // Lọc theo địa chỉ
        public DateTime? TuNgayDangKy { get; set; }   // Lọc từ ngày
        public DateTime? DenNgayDangKy { get; set; }  // Lọc đến ngày

        public List<KhachHang> DanhSachKhachHang { get; set; } = new();
    }
}