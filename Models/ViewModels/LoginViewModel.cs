// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: ViewModel dang nhap

using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;
    }
}