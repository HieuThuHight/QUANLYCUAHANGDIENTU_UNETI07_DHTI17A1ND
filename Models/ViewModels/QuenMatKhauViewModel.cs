// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: ViewModel quen mat khau

using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class QuenMatKhauViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email đăng ký")]
        public string Email { get; set; } = string.Empty;
    }
}