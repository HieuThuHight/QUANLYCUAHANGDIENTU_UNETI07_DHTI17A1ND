// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Entity Voucher - Chuc nang nang cao

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        public int MaVoucher { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [StringLength(50)]
        [Display(Name = "Mã voucher")]
        public string MaCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên voucher không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên voucher")]
        public string TenVoucher { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [Display(Name = "Loại giảm giá")]
        public string LoaiGiamGia { get; set; } = "PhanTram";

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải > 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giảm tối đa")]
        public decimal? GiamToiDa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn hàng tối thiểu")]
        public decimal DonHangToiThieu { get; set; } = 0;

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime NgayBatDau { get; set; }

        [Required]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime NgayKetThuc { get; set; }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; } = 100;

        [Display(Name = "Đã sử dụng")]
        public int DaSuDung { get; set; } = 0;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}