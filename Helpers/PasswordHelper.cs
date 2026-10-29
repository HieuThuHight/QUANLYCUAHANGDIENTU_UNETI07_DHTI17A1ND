// ============================================================================
// DỰ ÁN: QUẢN LÝ CỬA HÀNG ĐIỆN TỬ - UNETI (NHÓM 07 - DHTI17A1ND)
// PHÂN HỆ: PHẦN CHUNG (FOUNDATION & SHARED INFRASTRUCTURE)
// NHÓM SINH VIÊN THỰC HIỆN:
//   1. Đỗ Văn Hiếu    - 23203100018 - DHTI17A1ND - dvhieu.dhti17a1nd@sv.uneti.edu.vn (Trưởng nhóm)
//   2. Lưu Đức Huy    - 23274800002 - DHTI17A2ND - ldhuy.dhti17a2nd@sv.uneti.edu.vn
//   3. Vương Quang Đạt - 23203100036 - DHTI17A1ND - vqdat.dhti17a1nd@sv.uneti.edu.vn
//   4. Phạm Thái Hoàng - 23203100039 - DHTI17A1ND - pthoang.dhti17a1nd@sv.uneti.edu.vn
// NỘI DUNG: Mã hóa và kiểm tra băm mật khẩu bằng thuật toán an toàn BCrypt.Net
// ============================================================================

namespace QuanLyCuaHangDienTu.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return false;
            try { return BCrypt.Net.BCrypt.Verify(password, hashedPassword); }
            catch { return false; }
        }
    }
}