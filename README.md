# QuanLyCuaHangDienTu_UNETI07_DHTI17A1ND

## Giới thiệu
Đồ án ASP.NET Core 10 MVC - Hệ thống quản lý cửa hàng điện tử

## Thành viên
| STT | Họ tên | MSSV | Module |
|---|---|---|---|
| 1 | Đỗ Văn Hiếu | 23203100018 | Tài khoản, Loại SP |
| 2 | Lưu Đức Huy | 23274800002 | Sản phẩm, Voucher, Hoàn hàng |
| 3 | Vương Quang Đạt | 23203100036 | Giỏ hàng, Khách hàng |
| 4 | Phạm Thái Hoàng | 23203100039 | Đơn hàng, Thống kê |

## Công nghệ
- .NET 10 SDK
- ASP.NET Core 10 MVC
- Entity Framework Core 10
- SQL Server LocalDB
- Bootstrap 5, Chart.js
- BCrypt, ClosedXML, QuestPDF, MailKit

## Cài đặt
1. Clone repo
2. Mở `QuanLyCuaHangDienTu.sln` bằng VS 2026
3. Trong Package Manager Console: `Update-Database`
4. Nhấn F5

## Tài khoản mẫu
- Admin: `admin / admin123`
- Khách: `kh01 / khach123` (kh02...kh15)

## Tính năng
### Admin
- Quản lý loại SP, sản phẩm, khách hàng, đơn hàng
- Quản lý voucher, xử lý hoàn hàng
- Dashboard + Thống kê
- Xuất Excel/PDF

### Khách hàng
- Đăng ký, đăng nhập, quên mật khẩu
- Xem, tìm kiếm, lọc, sắp xếp sản phẩm
- Đặt hàng, áp dụng voucher
- Xem đơn hàng, yêu cầu hoàn hàng