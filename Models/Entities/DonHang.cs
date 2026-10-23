// Họ và tên: Phạm Thái Hoàng
// Mã sinh viên: 23203100039
// Nội dung thực hiện: Entity DonHang - Module 4

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        // DonHang.cs
        [StringLength(500)]
        [Display(Name = "Lý do hủy")]
        public string? LyDoHuy { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái hủy")]
        public string? TrangThaiHuy { get; set; }   // "ChoXacNhan", "DaHuy", "TuChoi"

        [Display(Name = "Ngày yêu cầu hủy")]
        public DateTime? NgayYeuCauHuy { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [StringLength(255)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giảm giá")]
        public decimal GiamGia { get; set; } = 0;

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "ChoXacNhan";

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public int? MaVoucher { get; set; }

        [ForeignKey("MaKhachHang")]
        public KhachHang? KhachHang { get; set; }

        [ForeignKey("MaVoucher")]
        public Voucher? Voucher { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public ICollection<YeuCauHoanHang> YeuCauHoanHangs { get; set; } = new List<YeuCauHoanHang>();
    }
}