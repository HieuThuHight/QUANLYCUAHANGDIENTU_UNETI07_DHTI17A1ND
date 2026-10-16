// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Helper Session

using System.Text.Json;

namespace QuanLyCuaHangDienTu.Helpers
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
            => session.SetString(key, JsonSerializer.Serialize(value));

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static bool IsAdmin(this HttpContext context)
            => context.Session.GetString("VaiTro") == "Admin";

        public static bool IsKhachHang(this HttpContext context)
            => context.Session.GetString("VaiTro") == "KhachHang";

        public static string? GetHoTen(this HttpContext context)
            => context.Session.GetString("HoTen");

        public static int? GetMaKhachHang(this HttpContext context)
            => context.Session.GetInt32("MaKhachHang");
    }
}