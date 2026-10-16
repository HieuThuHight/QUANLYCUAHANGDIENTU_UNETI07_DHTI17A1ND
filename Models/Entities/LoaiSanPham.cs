// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Entity LoaiSanPham - Module 1

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu.Models.Entities
{
    [Table("LoaiSanPham")]
    public class LoaiSanPham
    {
        [Key]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Tên loại không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên loại")]
        public string TenLoai { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}