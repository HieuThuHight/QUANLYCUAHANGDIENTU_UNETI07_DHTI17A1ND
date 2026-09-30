using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlycuahangdientu_uneti07_dhti17a1nd.Models.Entities
{
    [Table("LoaiSanPham")]
    public class LoaiSanPham
    {
        [Key]
        public int MaLoai { get; set; }

        [Required, StringLength(100)]
        public string TenLoai { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}