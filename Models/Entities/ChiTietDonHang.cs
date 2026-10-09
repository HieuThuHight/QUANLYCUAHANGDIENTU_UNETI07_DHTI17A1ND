using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("ChiTietDonHang")]
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTiet { get; set; }

        public int MaDonHang { get; set; }
        public int MaSanPham { get; set; }

        [Range(1, int.MaxValue)]
        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        // Navigation
        [ForeignKey("MaDonHang")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaSanPham")]
        public SanPham? SanPham { get; set; }
    }
}
