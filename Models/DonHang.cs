using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models
{
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }
        [Required]
        [Display(Name = "Khách hàng")]
        public int MaKhachHang { get; set; }
        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [StringLength(200)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiaoHang { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ xác nhận";
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
