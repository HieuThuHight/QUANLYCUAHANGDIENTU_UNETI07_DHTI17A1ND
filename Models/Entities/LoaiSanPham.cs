using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("LoaiSanPham")]
    public class LoaiSanPham
    {
        [Key]
        [Display(Name = "Mã loại")]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Tên loại không được để trống")]
        [StringLength(100, ErrorMessage = "Tên loại tối đa 100 ký tự")]
        [Display(Name = "Tên loại")]
        public string TenLoai { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Navigation — KHÔNG sửa, đây là code chung
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}