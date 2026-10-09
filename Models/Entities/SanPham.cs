
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        [Required, StringLength(200)]
        public string TenSanPham { get; set; } = string.Empty;

        public int MaLoai { get; set; }

        [Required, StringLength(100)]
        public string ThuongHieu { get; set; } = string.Empty;

        [StringLength(100)]
        public string? XuatXu { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal DonGia { get; set; }

        [Range(0, int.MaxValue)]
        public int SoLuongTon { get; set; }

        public int? ThoiGianBaoHanh { get; set; }

        [StringLength(1000)]
        public string? MoTa { get; set; }

        [StringLength(500)]
        public string? HinhAnh { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; } = "Đang kinh doanh";

        // Navigation
        [ForeignKey("MaLoai")]
        public LoaiSanPham? LoaiSanPham { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}