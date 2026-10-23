using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class YeuCauHuyDonViewModel
    {
        public int MaDonHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do hủy đơn.")]
        [StringLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự.")]
        public string LyDoHuy { get; set; } = string.Empty;
    }
}