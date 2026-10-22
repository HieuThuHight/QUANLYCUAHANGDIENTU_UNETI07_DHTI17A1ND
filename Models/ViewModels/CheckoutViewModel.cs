// Họ và tên: Vương Quang Đạt
// Mã sinh viên: 23203100036
// Nội dung thực hiện: ViewModel thanh toan

using System.ComponentModel.DataAnnotations;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [StringLength(255)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Mã voucher")]
        public string? MaCodeVoucher { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "COD";

        public List<Voucher> Vouchers { get; set; } = new();

        public decimal TongTien => Items.Sum(i => i.ThanhTien);
        public decimal GiamGia { get; set; } = 0;
        public decimal ThanhToan => TongTien - GiamGia;
    }
}
