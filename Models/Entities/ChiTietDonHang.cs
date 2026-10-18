// Họ và tên: Vương Quang Đạt
// Mã sinh viên: 23203100036
// Nội dung thực hiện: Entity ChiTietDonHang - Module 3

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("ChiTietDonHang")]
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTiet { get; set; }

        [Required]
        public int MaDonHang { get; set; }

        [Required]
        public int MaSanPham { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaDonHang")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaSanPham")]
        public SanPham? SanPham { get; set; }
    }
}
