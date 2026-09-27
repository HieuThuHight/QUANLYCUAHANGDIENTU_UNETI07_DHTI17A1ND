using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models
{
    public class ChiTietDonHang
    {
        [Display(Name = "Đơn hàng")]
        public int MaDonHang { get; set; }
        [ForeignKey(nameof(MaDonHang))]
        public DonHang? DonHang { get; set; }
        [Display(Name = "Sản phẩm")]
        public int MaSanPham { get; set; }
        [ForeignKey(nameof(MaSanPham))]
        public SanPham? SanPham { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng đặt phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }
    }
}
