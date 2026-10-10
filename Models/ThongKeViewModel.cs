// Họ và tên: Pham Thai Hoang
// Mã sinh viên: 23203100039
// Nội dung thực hiện: ViewModel cho trang thống kê
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models
{
    public class ThongKeItem
    {
        public string Ten { get; set; } = "";
        public int SoLuong { get; set; }
    }

    public class DoanhThuThang
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class ThongKeViewModel
    {
        public List<ThongKeItem> SanPhamTheoLoai { get; set; } = new();
        public List<ThongKeItem> DonTheoTrangThai { get; set; } = new();
        public List<ThongKeItem> SoLuongBanTheoSanPham { get; set; } = new();
        public ThongKeItem? SanPhamBanChayNhat { get; set; }
        public List<ThongKeItem> DonTheoKhachHang { get; set; } = new();
        public List<DoanhThuThang> DoanhThuTheoThang { get; set; } = new();
    }
}