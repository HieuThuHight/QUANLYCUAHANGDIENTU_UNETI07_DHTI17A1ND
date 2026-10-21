// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Xu ly voucher

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

            var v = await _context.Vouchers.FirstOrDefaultAsync(x => x.MaCode == maCode.Trim());

            if (v == null) return (false, 0, "Mã voucher không tồn tại.");
            if (!v.TrangThai) return (false, 0, "Voucher đã bị khóa.");
            if (v.NgayBatDau.Date > DateTime.Now.Date) return (false, 0, "Voucher chưa có hiệu lực.");
            if (v.NgayKetThuc.Date < DateTime.Now.Date) return (false, 0, "Voucher đã hết hạn.");
            if (v.SoLuong <= v.DaSuDung) return (false, 0, "Voucher đã hết lượt.");
            if (tongTien < v.DonHangToiThieu)
                return (false, 0, $"Đơn tối thiểu {v.DonHangToiThieu:N0}đ.");

            decimal giam;
            if (v.LoaiGiamGia == "PhanTram")
            {
                giam = tongTien * v.GiaTriGiam / 100m;
                if (v.GiamToiDa.HasValue && giam > v.GiamToiDa.Value)
                    giam = v.GiamToiDa.Value;
            }
            else giam = v.GiaTriGiam;

            if (giam < 0) giam = 0;
            if (giam > tongTien) giam = tongTien;

            return (true, giam, $"Áp dụng thành công, giảm {giam:N0}đ");
        }
    }
}