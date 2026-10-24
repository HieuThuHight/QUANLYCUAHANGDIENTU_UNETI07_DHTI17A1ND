// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Xu ly voucher - Chuc nang nang cao

using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;

namespace QuanLyCuaHangDienTu.Services
{
    public class VoucherService
    {
        private readonly AppDbContext _context;
        public VoucherService(AppDbContext context) => _context = context;

        public async Task<(bool ok, decimal giam, string message)> ApDungVoucher(string maCode, decimal tongTien)
        {
            if (string.IsNullOrWhiteSpace(maCode))
                return (false, 0, "Vui lòng nhập mã voucher.");

            var codeClean = maCode.Trim().ToUpper();

            // Tìm mã không phân biệt hoa thường
            var v = await _context.Vouchers.FirstOrDefaultAsync(x => x.MaCode.ToUpper() == codeClean);

            if (v == null) 
                return (false, 0, $"Mã voucher '{maCode.Trim()}' không tồn tại trong hệ thống.");

            if (!v.TrangThai) 
                return (false, 0, $"Mã voucher '{v.MaCode}' hiện đang tạm khóa.");

            if (v.NgayBatDau.Date > DateTime.Now.Date) 
                return (false, 0, $"Mã '{v.MaCode}' có hiệu lực từ ngày {v.NgayBatDau:dd/MM/yyyy}.");

            if (v.NgayKetThuc.Date < DateTime.Now.Date) 
                return (false, 0, $"Mã '{v.MaCode}' đã hết hạn từ ngày {v.NgayKetThuc:dd/MM/yyyy}.");

            if (v.SoLuong <= v.DaSuDung) 
                return (false, 0, $"Mã '{v.MaCode}' đã hết lượt sử dụng ({v.DaSuDung}/{v.SoLuong}).");

            if (tongTien < v.DonHangToiThieu)
            {
                var thieu = v.DonHangToiThieu - tongTien;
                return (false, 0, $"Mã '{v.MaCode}' chỉ áp dụng cho đơn từ {v.DonHangToiThieu:N0} đ (bạn cần mua thêm {thieu:N0} đ).");
            }

            decimal giam;
            if (v.LoaiGiamGia == "PhanTram")
            {
                giam = Math.Round(tongTien * v.GiaTriGiam / 100m, 0);
                if (v.GiamToiDa.HasValue && giam > v.GiamToiDa.Value)
                    giam = v.GiamToiDa.Value;
            }
            else
            {
                giam = v.GiaTriGiam;
            }

            if (giam < 0) giam = 0;

            // Đảm bảo đơn còn tối thiểu 1.000đ để có thể test VietQR nếu chuyển khoản
            if (tongTien > 1000 && giam >= tongTien)
            {
                giam = tongTien - 1000;
            }
            else if (giam > tongTien)
            {
                giam = tongTien;
            }

            return (true, giam, $"Áp dụng thành công mã {v.MaCode}: Giảm {giam:N0} đ!");
        }
    }
}
