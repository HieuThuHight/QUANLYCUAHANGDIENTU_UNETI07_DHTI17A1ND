// Họ và tên: Pham Thai Hoang
// Mã sinh viên: 23203100039
// Nội dung thực hiện: Quy tắc chuyển trạng thái đơn hàng
namespace quanlycuahangdientu_uneti07_dhti17a1nd.Helpers
{
    public static class TrangThaiDonHang
    {
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DaXacNhan = "Đã xác nhận";
        public const string DangGiao = "Đang giao";
        public const string DaHoanThanh = "Đã hoàn thành";
        public const string DaHuy = "Đã hủy";

        public static bool ChuyenDuoc(string hienTai, string moi)
        {
            return (hienTai, moi) switch
            {
                (ChoXacNhan, DaXacNhan) => true,
                (DaXacNhan, DangGiao) => true,
                (DangGiao, DaHoanThanh) => true,
                (ChoXacNhan, DaHuy) => true,
                (DaXacNhan, DaHuy) => true,
                _ => false
            };
        }
    }
}