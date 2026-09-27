using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models
{
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(150)]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;
        [Display(Name = "Loại sản phẩm")]
        public int MaLoai { get; set; }
        [ForeignKey(nameof(MaLoai))]
        public LoaiSanPham? LoaiSanPham { get; set; }
        [Required(ErrorMessage = "Thương hiệu không được để trống")]
        [StringLength(100)]
        [Display(Name = "Thương hiệu")]
        public string ThuongHieu { get; set; } = string.Empty;
        [StringLength(100)]
        [Display(Name = "Xuất xứ")]
        public string? XuatXu { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }
        [Display(Name = "Thời gian bảo hành (tháng)")]
        public int? ThoiGianBaoHanh { get; set; }
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang kinh doanh";
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
