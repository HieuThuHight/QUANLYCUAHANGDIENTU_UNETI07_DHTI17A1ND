using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required, StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string MatKhau { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string VaiTro { get; set; } = "KhachHang";

        public bool TrangThai { get; set; } = true;

        // Navigation
        public KhachHang? KhachHang { get; set; }
    }
}
