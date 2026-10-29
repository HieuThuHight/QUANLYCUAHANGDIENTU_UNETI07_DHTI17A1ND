# QuanLyCuaHangDienTu_UNETI07_DHTI17A1ND

## Giới thiệu
Đồ án ASP.NET Core 10 MVC - Hệ thống quản lý cửa hàng điện tử
Trường Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)

## Thành viên nhóm thực hiện
| STT | Họ tên | MSSV | Lớp | Email UNETI | Vai trò & Module phụ trách |
|---|---|---|---|---|---|
| 1 | Đỗ Văn Hiếu | 23203100018 | DHTI17A1ND | dvhieu.dhti17a1nd@sv.uneti.edu.vn | Trưởng nhóm - Module 1: Xác thực, Phân quyền, Tài khoản & Loại SP |
| 2 | Lưu Đức Huy | 23274800002 | DHTI17A2ND | ldhuy.dhti17a2nd@sv.uneti.edu.vn | Thành viên - Module 2: Sản phẩm, Voucher, Hoàn hàng & Báo cáo Excel/PDF |
| 3 | Vương Quang Đạt | 23203100036 | DHTI17A1ND | vqdat.dhti17a1nd@sv.uneti.edu.vn | Thành viên - Module 3: Giỏ hàng, Đặt hàng, Thanh toán VietQR/COD & Khách hàng |
| 4 | Phạm Thái Hoàng | 23203100039 | DHTI17A1ND | pthoang.dhti17a1nd@sv.uneti.edu.vn | Thành viên - Module 4: Quản lý đơn hàng, Vòng đời trạng thái & Dashboard |

## Phân chia Nhánh Git & Module
- **`main`**: Nhánh chính tích hợp toàn bộ hệ thống
- **`feature/module1-hieu`**: Module 1 - Đỗ Văn Hiếu (Tài khoản, Xác thực BCrypt, Quên mật khẩu, Loại sản phẩm, Trang chủ)
- **`feature/module2-huy`**: Module 2 - Lưu Đức Huy (CRUD Sản phẩm, Upload ảnh wwwroot, Kiểm tra trùng tên, Đơn giá > 0, Voucher, Hoàn hàng, Xuất Excel/PDF)
- **`feature/module3-dat`**: Module 3 - Vương Quang Đạt (Giỏ hàng Session, Đặt hàng chống hack giá, Thanh toán VietQR Napas247 & COD, Quản lý khách hàng, SeedData)
- **`feature/module4-hoang`**: Module 4 - Phạm Thái Hoàng (Quản lý đơn hàng admin, Vòng đời đơn, Live Polling đa client, Hủy đơn hoàn kho Serializable, Dashboard Chart.js)

## Công nghệ sử dụng
- .NET 10 SDK (C# 13)
- ASP.NET Core 10 MVC
- Entity Framework Core 10 (SQL Server LocalDB)
- Bootstrap 5.3, FontAwesome 6, Chart.js, GSAP Animations
- BCrypt.Net, ClosedXML, QuestPDF, MailKit

## Cài đặt & Khởi chạy
1. Clone repo: `git clone https://github.com/HieuThuHight/QUANLYCUAHANGDIENTU_UNETI07_DHTI17A1ND.git`
2. Mở `QuanLyCuaHangDienTu.sln` bằng Visual Studio
3. Trong Package Manager Console: `Update-Database`
4. Nhấn `F5` hoặc chạy lệnh: `dotnet run`

## Tài khoản mẫu hệ thống
- **Quản trị viên (Admin)**: `admin / admin123`
- **Khách hàng (Client)**: `kh01 / khach123` (đến `kh15 / khach123`)
