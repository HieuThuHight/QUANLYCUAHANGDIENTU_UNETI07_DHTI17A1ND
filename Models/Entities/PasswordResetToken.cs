// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Entity luu token dat lai mat khau

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("PasswordResetToken")]
    public class PasswordResetToken
    {
        [Key]
        public int MaToken { get; set; }

        [Required]
        public int MaTaiKhoan { get; set; }

        [Required]
        [StringLength(100)]
        public string Token { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayHetHan { get; set; }
        public bool DaSuDung { get; set; } = false;

        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan? TaiKhoan { get; set; }
    }
}