using quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models
{
    public class DashboardViewModel
    {
        public int TongLoaiSanPham { get; set; }
        public int TongSanPham { get; set; }
        public int TongKhachHang { get; set; }
        public int TongDonHang { get; set; }
        public int DonChoXacNhan { get; set; }
        public int DonDangGiao { get; set; }
        public int DonHoanThanh { get; set; }
        public int SanPhamDangKinhDoanh { get; set; }

        public List<SanPham> DanhSachSanPham { get; set; } = new();
    }
}