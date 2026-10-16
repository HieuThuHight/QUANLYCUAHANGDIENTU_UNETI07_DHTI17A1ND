// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Entity SanPham - Module 2

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        [Display(Name = "Loại sản phẩm")]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Thương hiệu không được để trống")]
        [StringLength(100)]
        [Display(Name = "Thương hiệu")]
        public string ThuongHieu { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Xuất xứ")]
        public string? XuatXu { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải >= 0")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [Range(0, 120, ErrorMessage = "Bảo hành từ 0-120 tháng")]
        [Display(Name = "Bảo hành (tháng)")]
        public int ThoiGianBaoHanh { get; set; }

        [StringLength(2000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaLoai")]
        public LoaiSanPham? LoaiSanPham { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}