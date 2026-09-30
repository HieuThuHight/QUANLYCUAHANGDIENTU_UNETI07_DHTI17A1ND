using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        public int MaTaiKhoan { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        [StringLength(15), Phone]
        public string? SoDienThoai { get; set; }

        [StringLength(100), EmailAddress]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Navigation
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan? TaiKhoan { get; set; }

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}
