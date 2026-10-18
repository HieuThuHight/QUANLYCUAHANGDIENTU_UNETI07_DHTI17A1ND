// Họ và tên: Vương Quang Đạt
// Mã sinh viên: 23203100036
// Nội dung thực hiện: ViewModel item gio hang

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string? HinhAnh { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }
}
