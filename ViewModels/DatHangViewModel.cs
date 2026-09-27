using System.ComponentModel.DataAnnotations;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.ViewModels
{
    public class DatHangViewModel
    {
        public int MaSanPham { get; set; }
        [Display(Name = "Sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng đặt phải lớn hơn 0")]
        [Display(Name = "Số lượng đặt")]
        public int SoLuong { get; set; } = 1;
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [StringLength(200)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; } = string.Empty;
    }
}
