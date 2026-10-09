/* ============================================================
   CSDL: Quản lý cửa hàng điện tử - SQL Server
   Project: QuanLyCuaHangDienTu_UNETI04_TI17A1HN
   ============================================================ */

IF DB_ID(N'QuanLyCuaHangDienTu') IS NOT NULL
    DROP DATABASE QuanLyCuaHangDienTu;
GO
CREATE DATABASE QuanLyCuaHangDienTu;
GO
USE QuanLyCuaHangDienTu;
GO

/* ============================ 1. TAIKHOAN ============================ */
CREATE TABLE TaiKhoan (
    MaTaiKhoan   INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap  NVARCHAR(50)  NOT NULL UNIQUE,
    MatKhau      NVARCHAR(255) NOT NULL,
    HoTen        NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(100) NOT NULL UNIQUE,
    VaiTro       NVARCHAR(20)  NOT NULL,   -- 'Admin' | 'KhachHang'
    TrangThai    BIT           NOT NULL DEFAULT 1
);
GO

/* ============================ 2. LOAISANPHAM ============================ */
CREATE TABLE LoaiSanPham (
    MaLoai     INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai    NVARCHAR(100) NOT NULL UNIQUE,
    MoTa       NVARCHAR(500) NULL,
    TrangThai  BIT           NOT NULL DEFAULT 1
);
GO

/* ============================ 3. SANPHAM ============================ */
CREATE TABLE SanPham (
    MaSanPham        INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham       NVARCHAR(200) NOT NULL,
    MaLoai           INT           NOT NULL,
    ThuongHieu       NVARCHAR(100) NOT NULL,
    XuatXu           NVARCHAR(100) NULL,
    DonGia           DECIMAL(18,2) NOT NULL,
    SoLuongTon       INT           NOT NULL DEFAULT 0,
    ThoiGianBaoHanh  INT           NULL,           -- số tháng
    MoTa             NVARCHAR(1000) NULL,
    HinhAnh          NVARCHAR(500) NULL,           -- [MỚI] đường dẫn ảnh
    TrangThai        NVARCHAR(30)  NOT NULL DEFAULT N'Đang kinh doanh',
    CONSTRAINT FK_SanPham_LoaiSanPham FOREIGN KEY (MaLoai) REFERENCES LoaiSanPham(MaLoai),
    CONSTRAINT CK_SanPham_DonGia      CHECK (DonGia > 0),
    CONSTRAINT CK_SanPham_SoLuongTon  CHECK (SoLuongTon >= 0)
);
GO
CREATE INDEX IX_SanPham_TenSanPham  ON SanPham(TenSanPham);
CREATE INDEX IX_SanPham_ThuongHieu  ON SanPham(ThuongHieu);
CREATE INDEX IX_SanPham_TrangThai   ON SanPham(TrangThai);
CREATE INDEX IX_SanPham_DonGia      ON SanPham(DonGia);  -- [MỚI] hỗ trợ lọc khoảng giá
GO

/* ============================ 4. KHACHHANG ============================ */
CREATE TABLE KhachHang (
    MaKhachHang   INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan    INT           NOT NULL UNIQUE,
    HoTen         NVARCHAR(100) NOT NULL,
    NgaySinh      DATE          NULL,
    GioiTinh      NVARCHAR(10)  NULL,
    SoDienThoai   NVARCHAR(15)  NULL,
    Email         NVARCHAR(100) NULL,
    DiaChi        NVARCHAR(255) NULL,
    NgayDangKy    DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    TrangThai     BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_KhachHang_TaiKhoan FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO

/* ============================ 5. DONHANG ============================ */
CREATE TABLE DonHang (
    MaDonHang         INT IDENTITY(1,1) PRIMARY KEY,
    MaKhachHang       INT           NOT NULL,
    NgayDat           DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    DiaChiGiaoHang    NVARCHAR(255) NOT NULL,
    TongTien          DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai         NVARCHAR(30)  NOT NULL DEFAULT N'Chờ xác nhận',
    NgayCapNhat       DATETIME2     NULL,   -- [MỚI] lần đổi trạng thái gần nhất
    GhiChu            NVARCHAR(500) NULL,   -- [MỚI] ghi chú đơn hàng
    CONSTRAINT FK_DonHang_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang)
);
GO
CREATE INDEX IX_DonHang_TrangThai ON DonHang(TrangThai);
CREATE INDEX IX_DonHang_NgayDat   ON DonHang(NgayDat);
GO

/* ============================ 6. CHITIETDONHANG ============================ */
CREATE TABLE ChiTietDonHang (
    MaChiTiet   INT IDENTITY(1,1) PRIMARY KEY,
    MaDonHang   INT           NOT NULL,
    MaSanPham   INT           NOT NULL,
    SoLuong     INT           NOT NULL,
    DonGia      DECIMAL(18,2) NOT NULL,     -- giá tại thời điểm đặt
    ThanhTien   DECIMAL(18,2) NOT NULL,     -- C# tự tính = SoLuong * DonGia
    CONSTRAINT UQ_ChiTiet_DonHang_SanPham UNIQUE (MaDonHang, MaSanPham),
    CONSTRAINT FK_ChiTiet_DonHang_DonHang FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTiet_DonHang_SanPham FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    CONSTRAINT CK_ChiTiet_SoLuong CHECK (SoLuong > 0)
);
GO

/* ============================ 7. GIOHANG [MỚI] ============================ */
CREATE TABLE GioHang (
    MaGioHang     INT IDENTITY(1,1) PRIMARY KEY,
    MaKhachHang   INT       NOT NULL,
    NgayTao       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    TrangThai     NVARCHAR(30) NOT NULL DEFAULT N'Đang dùng', -- Đang dùng / Đã đặt / Đã hủy
    CONSTRAINT FK_GioHang_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang)
);
GO

/* ============================ 8. GIOHANGCHITIET [MỚI] ============================ */
CREATE TABLE GioHangChiTiet (
    MaGioHangChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    MaGioHang        INT           NOT NULL,
    MaSanPham        INT           NOT NULL,
    SoLuong          INT           NOT NULL,
    DonGia           DECIMAL(18,2) NOT NULL,
    CONSTRAINT UQ_GioHang_SP UNIQUE (MaGioHang, MaSanPham),
    CONSTRAINT FK_GioHangChiTiet_GioHang FOREIGN KEY (MaGioHang) REFERENCES GioHang(MaGioHang) ON DELETE CASCADE,
    CONSTRAINT FK_GioHangChiTiet_SanPham FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    CONSTRAINT CK_GioHangChiTiet_SoLuong CHECK (SoLuong > 0)
);
GO

/* ============================ 9. NHATKY [MỚI] ============================ */
CREATE TABLE NhatKy (
    MaNhatKy    INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan  INT           NULL,
    HanhDong    NVARCHAR(50)  NOT NULL,   -- Them / Sua / Xoa / DangNhap ...
    DoiTuong    NVARCHAR(100) NULL,       -- SanPham / DonHang ...
    MaDoiTuong  INT           NULL,
    NoiDung     NVARCHAR(500) NULL,
    ThoiGian    DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_NhatKy_TaiKhoan FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO

/* ============================================================
   DỮ LIỆU MẪU
   ============================================================ */

-- TaiKhoan (thêm 1 TK bị khóa id=17 để test)
SET IDENTITY_INSERT TaiKhoan ON;
INSERT INTO TaiKhoan (MaTaiKhoan,TenDangNhap,MatKhau,HoTen,Email,VaiTro,TrangThai) VALUES
(1,'admin','123456',N'Quản Trị Viên','admin@uneti.edu.vn','Admin',1),
(2,'khachhang1','123456',N'Nguyễn Văn A','nva@gmail.com','KhachHang',1),
(3,'khachhang2','123456',N'Trần Thị B','ttb@gmail.com','KhachHang',1),
(4,'khachhang3','123456',N'Lê Văn C','lvc@gmail.com','KhachHang',1),
(5,'khachhang4','123456',N'Phạm Thị D','ptd@gmail.com','KhachHang',1),
(6,'khachhang5','123456',N'Hoàng Văn E','hve@gmail.com','KhachHang',1),
(7,'khachhang6','123456',N'Vũ Thị F','vtf@gmail.com','KhachHang',1),
(8,'khachhang7','123456',N'Đặng Văn G','dvg@gmail.com','KhachHang',1),
(9,'khachhang8','123456',N'Bùi Thị H','bth@gmail.com','KhachHang',1),
(10,'khachhang9','123456',N'Đỗ Văn I','dvi@gmail.com','KhachHang',1),
(11,'khachhang10','123456',N'Ngô Thị K','ntk@gmail.com','KhachHang',1),
(12,'khachhang11','123456',N'Lý Văn L','lvl@gmail.com','KhachHang',1),
(13,'khachhang12','123456',N'Trịnh Thị M','ttm@gmail.com','KhachHang',1),
(14,'khachhang13','123456',N'Phan Văn N','pvn@gmail.com','KhachHang',1),
(15,'khachhang14','123456',N'Dương Thị O','dto@gmail.com','KhachHang',1),
(16,'khachhang15','123456',N'Đinh Văn P','dvp@gmail.com','KhachHang',1),
(17,'khachhang16','123456',N'Bị Khóa Test','khoa@gmail.com','KhachHang',0); -- [MỚI] test khóa
SET IDENTITY_INSERT TaiKhoan OFF;

-- LoaiSanPham
SET IDENTITY_INSERT LoaiSanPham ON;
INSERT INTO LoaiSanPham (MaLoai,TenLoai,MoTa,TrangThai) VALUES
(1,N'Điện thoại',N'Các loại điện thoại thông minh',1),
(2,N'Laptop',N'Máy tính xách tay văn phòng và gaming',1),
(3,N'Tai nghe',N'Tai nghe nhạc, tai nghe gaming',1),
(4,N'Chuột',N'Chuột máy tính có dây và không dây',1),
(5,N'Bàn phím',N'Bàn phím cơ, bàn phím văn phòng',1);
SET IDENTITY_INSERT LoaiSanPham OFF;

-- SanPham (thêm SP 21: Đang kinh doanh nhưng tồn = 0 để test)
SET IDENTITY_INSERT SanPham ON;
INSERT INTO SanPham (MaSanPham,TenSanPham,MaLoai,ThuongHieu,XuatXu,DonGia,SoLuongTon,ThoiGianBaoHanh,MoTa,HinhAnh,TrangThai) VALUES
(1,N'iPhone 15 Pro Max 256GB',1,'Apple',N'Mỹ',30000000,50,12,N'Điện thoại cao cấp nhất của Apple',NULL,N'Đang kinh doanh'),
(2,N'Samsung Galaxy S24 Ultra',1,'Samsung',N'Hàn Quốc',28000000,40,12,N'Flagship Android mạnh mẽ',NULL,N'Đang kinh doanh'),
(3,N'Xiaomi 14 Pro',1,'Xiaomi',N'Trung Quốc',15000000,60,12,N'Điện thoại cấu hình cao giá rẻ',NULL,N'Đang kinh doanh'),
(4,N'Oppo Reno 11',1,'Oppo',N'Trung Quốc',10000000,80,12,N'Thiết kế đẹp, camera tốt',NULL,N'Đang kinh doanh'),
(5,N'MacBook Air M3',2,'Apple',N'Mỹ',25000000,30,12,N'Laptop mỏng nhẹ, pin trâu',NULL,N'Đang kinh doanh'),
(6,N'Dell XPS 13',2,'Dell',N'Mỹ',22000000,25,12,N'Laptop cao cấp cho doanh nhân',NULL,N'Đang kinh doanh'),
(7,N'Asus ROG Strix G15',2,'Asus',N'Đài Loan',20000000,35,24,N'Laptop gaming mạnh mẽ',NULL,N'Đang kinh doanh'),
(8,N'HP Pavilion 15',2,'HP',N'Mỹ',15000000,45,12,N'Laptop văn phòng phổ thông',NULL,N'Đang kinh doanh'),
(9,N'Lenovo ThinkPad X1',2,'Lenovo',N'Trung Quốc',26000000,20,12,N'Bàn phím tốt, bền bỉ',NULL,N'Đang kinh doanh'),
(10,N'Sony WH-1000XM5',3,'Sony',N'Nhật Bản',6000000,100,12,N'Tai nghe chống ồn tốt nhất',NULL,N'Đang kinh doanh'),
(11,N'AirPods Pro 2',3,'Apple',N'Mỹ',5500000,150,12,N'Tai nghe true wireless',NULL,N'Đang kinh doanh'),
(12,N'Logitech G Pro X',3,'Logitech',N'Thụy Sĩ',3000000,80,12,N'Tai nghe gaming',NULL,N'Tạm ngừng'),
(13,N'Razer BlackShark V2',3,'Razer',N'Mỹ',2500000,60,12,N'Tai nghe gaming giá rẻ',NULL,N'Đang kinh doanh'),
(14,N'Logitech MX Master 3S',4,'Logitech',N'Thụy Sĩ',2000000,120,12,N'Chuột không dây cao cấp',NULL,N'Đang kinh doanh'),
(15,N'Razer DeathAdder V3',4,'Razer',N'Mỹ',1800000,90,12,N'Chuột gaming',NULL,N'Đang kinh doanh'),
(16,N'Chuột Bluetooth Xiaomi',4,'Xiaomi',N'Trung Quốc',300000,200,6,N'Chuột văn phòng giá rẻ',NULL,N'Đang kinh doanh'),
(17,N'Bàn phím cơ Akko 3068B',5,'Akko',N'Trung Quốc',1500000,70,12,N'Bàn phím cơ nhỏ gọn',NULL,N'Đang kinh doanh'),
(18,N'Keychron K8 Pro',5,'Keychron',N'Trung Quốc',2200000,50,12,N'Bàn phím cơ không dây',NULL,N'Đang kinh doanh'),
(19,N'Bàn phím Logitech K380',5,'Logitech',N'Thụy Sĩ',700000,100,12,N'Bàn phím Bluetooth',NULL,N'Đang kinh doanh'),
(20,N'Bàn phím cơ Razer Huntsman',5,'Razer',N'Mỹ',3500000,0,12,N'Bàn phím gaming cao cấp',NULL,N'Ngừng kinh doanh'),
(21,N'Tai nghe Sony WF-1000XM5',3,'Sony',N'Nhật Bản',5000000,0,12,N'Đang kinh doanh nhưng hết hàng',NULL,N'Đang kinh doanh'); -- [MỚI]
SET IDENTITY_INSERT SanPham OFF;

-- KhachHang
SET IDENTITY_INSERT KhachHang ON;
INSERT INTO KhachHang (MaKhachHang,MaTaiKhoan,HoTen,NgaySinh,GioiTinh,SoDienThoai,Email,DiaChi,NgayDangKy,TrangThai) VALUES
(1,2,N'Nguyễn Văn A','1990-01-01',N'Nam','0912345678','nva@gmail.com',N'Hà Nội','2025-01-10',1),
(2,3,N'Trần Thị B','1995-05-15',N'Nữ','0912345679','ttb@gmail.com',N'TP.HCM','2025-01-12',1),
(3,4,N'Lê Văn C','1988-10-20',N'Nam','0912345680','lvc@gmail.com',N'Đà Nẵng','2025-01-15',1),
(4,5,N'Phạm Thị D','2000-02-02',N'Nữ','0912345681','ptd@gmail.com',N'Hải Phòng','2025-02-01',1),
(5,6,N'Hoàng Văn E','1992-12-12',N'Nam','0912345682','hve@gmail.com',N'Cần Thơ','2025-02-10',1),
(6,7,N'Vũ Thị F','1998-08-08',N'Nữ','0912345683','vtf@gmail.com',N'Hà Nội','2025-02-15',1),
(7,8,N'Đặng Văn G','1991-03-03',N'Nam','0912345684','dvg@gmail.com',N'TP.HCM','2025-03-01',1),
(8,9,N'Bùi Thị H','1996-07-07',N'Nữ','0912345685','bth@gmail.com',N'Đà Nẵng','2025-03-05',1),
(9,10,N'Đỗ Văn I','1994-11-11',N'Nam','0912345686','dvi@gmail.com',N'Hải Phòng','2025-03-10',1),
(10,11,N'Ngô Thị K','1999-09-09',N'Nữ','0912345687','ntk@gmail.com',N'Cần Thơ','2025-03-20',1),
(11,12,N'Lý Văn L','1993-04-04',N'Nam','0912345688','lvl@gmail.com',N'Hà Nội','2025-04-01',1),
(12,13,N'Trịnh Thị M','1997-06-06',N'Nữ','0912345689','ttm@gmail.com',N'TP.HCM','2025-04-10',1),
(13,14,N'Phan Văn N','1989-01-01',N'Nam','0912345690','pvn@gmail.com',N'Đà Nẵng','2025-04-15',1),
(14,15,N'Dương Thị O','2001-02-02',N'Nữ','0912345691','dto@gmail.com',N'Hải Phòng','2025-05-01',1),
(15,16,N'Đinh Văn P','1990-10-10',N'Nam','0912345692','dvp@gmail.com',N'Cần Thơ','2025-05-10',1);
SET IDENTITY_INSERT KhachHang OFF;

-- DonHang
SET IDENTITY_INSERT DonHang ON;
INSERT INTO DonHang (MaDonHang,MaKhachHang,NgayDat,DiaChiGiaoHang,TongTien,TrangThai,NgayCapNhat) VALUES
(1,1,'2025-10-01 08:00:00',N'Hà Nội',36000000,N'Đã hoàn thành','2025-10-05 10:00:00'),
(2,2,'2025-10-05 09:30:00',N'TP.HCM',4000000,N'Đã hoàn thành','2025-10-08 14:00:00'),
(3,3,'2025-10-10 10:00:00',N'Đà Nẵng',22000000,N'Đã hủy','2025-10-12 09:00:00'),
(4,4,'2025-11-01 14:00:00',N'Hải Phòng',29500000,N'Đang giao','2025-11-03 08:00:00'),
(5,5,'2025-11-15 15:30:00',N'Cần Thơ',25000000,N'Đã xác nhận','2025-11-16 10:00:00'),
(6,6,'2025-12-01 08:15:00',N'Hà Nội',900000,N'Chờ xác nhận',NULL),
(7,7,'2025-12-10 16:45:00',N'TP.HCM',5500000,N'Đã hoàn thành','2025-12-15 12:00:00'),
(8,8,'2026-01-05 09:00:00',N'Đà Nẵng',17200000,N'Đang giao','2026-01-06 09:00:00'),
(9,9,'2026-01-20 13:20:00',N'Hải Phòng',20000000,N'Đã xác nhận','2026-01-21 09:00:00'),
(10,10,'2026-02-01 10:10:00',N'Cần Thơ',5000000,N'Chờ xác nhận',NULL),
(11,11,'2026-02-15 14:50:00',N'Hà Nội',11800000,N'Đã hoàn thành','2026-02-20 10:00:00'),
(12,12,'2026-03-01 08:30:00',N'TP.HCM',15000000,N'Đã hủy','2026-03-02 09:00:00'),
(13,13,'2026-03-10 11:00:00',N'Đà Nẵng',26700000,N'Đang giao','2026-03-11 09:00:00'),
(14,14,'2026-03-20 15:00:00',N'Hải Phòng',3000000,N'Đã xác nhận','2026-03-21 09:00:00'),
(15,15,'2026-03-25 09:00:00',N'Cần Thơ',35500000,N'Chờ xác nhận',NULL);
SET IDENTITY_INSERT DonHang OFF;

-- ChiTietDonHang (ThanhTien nhập thủ công = SoLuong*DonGia)
SET IDENTITY_INSERT ChiTietDonHang ON;
INSERT INTO ChiTietDonHang (MaChiTiet,MaDonHang,MaSanPham,SoLuong,DonGia,ThanhTien) VALUES
(1,1,1,1,30000000,30000000),
(2,1,10,1,6000000,6000000),
(3,2,14,2,2000000,4000000),
(4,3,6,1,22000000,22000000),
(5,4,2,1,28000000,28000000),
(6,4,17,1,1500000,1500000),
(7,5,5,1,25000000,25000000),
(8,6,16,3,300000,900000),
(9,7,11,1,5500000,5500000),
(10,8,3,1,15000000,15000000),
(11,8,18,1,2200000,2200000),
(12,9,7,1,20000000,20000000),
(13,10,13,2,2500000,5000000),
(14,11,4,1,10000000,10000000),
(15,11,15,1,1800000,1800000),
(16,12,8,1,15000000,15000000),
(17,13,9,1,26000000,26000000),
(18,13,19,1,700000,700000),
(19,14,12,1,3000000,3000000),
(20,15,1,1,30000000,30000000),
(21,15,11,1,5500000,5500000);
SET IDENTITY_INSERT ChiTietDonHang OFF;

-- NhatKy mẫu (tùy chọn)
INSERT INTO NhatKy (MaTaiKhoan,HanhDong,DoiTuong,MaDoiTuong,NoiDung) VALUES
(1,'DangNhap','TaiKhoan',1,N'Admin đăng nhập hệ thống'),
(1,'XacNhan','DonHang',5,N'Xác nhận đơn hàng #5');
GO

PRINT N'>>> Tạo CSDL QuanLyCuaHangDienTu thành công!';
GO