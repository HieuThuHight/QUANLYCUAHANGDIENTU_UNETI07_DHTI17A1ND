// ============================================================================
// DỰ ÁN: QUẢN LÝ CỬA HÀNG ĐIỆN TỬ - UNETI (NHÓM 07 - DHTI17A1ND)
// PHÂN HỆ: PHẦN CHUNG (FOUNDATION & SHARED INFRASTRUCTURE)
// NHÓM SINH VIÊN THỰC HIỆN:
//   1. Đỗ Văn Hiếu    - 23203100018 - DHTI17A1ND - dvhieu.dhti17a1nd@sv.uneti.edu.vn (Trưởng nhóm)
//   2. Lưu Đức Huy    - 23274800002 - DHTI17A2ND - ldhuy.dhti17a2nd@sv.uneti.edu.vn
//   3. Vương Quang Đạt - 23203100036 - DHTI17A1ND - vqdat.dhti17a1nd@sv.uneti.edu.vn
//   4. Phạm Thái Hoàng - 23203100039 - DHTI17A1ND - pthoang.dhti17a1nd@sv.uneti.edu.vn
// NỘI DUNG: Phương thức mở rộng ISession hỗ trợ lưu trữ và đọc đối tượng JSON phức tạp
// ============================================================================

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