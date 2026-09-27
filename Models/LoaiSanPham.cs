using System.ComponentModel.DataAnnotations;
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models
{
    public class LoaiSanPham
    {
        [Key]
        public int MaLoai { get; set; }
        [Required(ErrorMessage = "Tên loại không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên loại")]
        public string TenLoai { get; set; } = string.Empty;
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
