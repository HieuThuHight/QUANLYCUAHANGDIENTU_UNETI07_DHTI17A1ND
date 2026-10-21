// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Entity YeuCauHoanHang - Chuc nang nang cao

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("YeuCauHoanHang")]
    public class YeuCauHoanHang
    {
        [Key]
        public int MaYeuCau { get; set; }

        [Required]
        public int MaDonHang { get; set; }

        [Required(ErrorMessage = "Lý do không được để trống")]
        [StringLength(1000)]
        [Display(Name = "Lý do hoàn hàng")]
        public string LyDo { get; set; } = string.Empty;

        [Display(Name = "Ngày yêu cầu")]
        public DateTime NgayYeuCau { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "ChoDuyet";

        [StringLength(500)]
        [Display(Name = "Phản hồi của Admin")]
        public string? PhanHoiAdmin { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền hoàn")]
        public decimal SoTienHoan { get; set; } = 0;

        [Display(Name = "Ngày xử lý")]
        public DateTime? NgayXuLy { get; set; }

        [ForeignKey("MaDonHang")]
        public DonHang? DonHang { get; set; }
    }
}