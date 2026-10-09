using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        public int MaKhachHang { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required, StringLength(255)]
        public string DiaChiGiaoHang { get; set; } = string.Empty;

        public decimal TongTien { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; } = "Chờ xác nhận";

        public DateTime? NgayCapNhat { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        // Navigation
        [ForeignKey("MaKhachHang")]
        public KhachHang? KhachHang { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
