CREATE DATABASE QuanLyTourDuLich
GO

USE QuanLyTourDuLich
GO

------------------------------------------------
-- ACCOUNTS
------------------------------------------------
CREATE TABLE Accounts
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE,
    Password NVARCHAR(100),
    Phone NVARCHAR(20),
    VaiTro NVARCHAR(50)
)

------------------------------------------------
-- KHÁCH HÀNG
------------------------------------------------
CREATE TABLE KhachHangs
(
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100),
    NgaySinh DATE NULL,
    SoDienThoai NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    DiaChi NVARCHAR(200) NULL,
    AccountId INT,

    FOREIGN KEY (AccountId) 
    REFERENCES Accounts(Id)
    ON DELETE CASCADE
)

------------------------------------------------
-- NHÂN VIÊN
------------------------------------------------
CREATE TABLE NhanViens
(
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    TenNV NVARCHAR(100) NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL,
    SoDienThoai NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    DiaChi NVARCHAR(255) NULL,
    ChucVu NVARCHAR(50) NULL,
    AccountId INT,

    FOREIGN KEY (AccountId)
    REFERENCES Accounts(Id)
    ON DELETE CASCADE
)

------------------------------------------------
-- TOUR
------------------------------------------------
CREATE TABLE TourDuLiches
(
    MaTour INT IDENTITY(1,1) PRIMARY KEY,
    TenTour NVARCHAR(150),
    GiaVe FLOAT,
    ThoiGianDi NVARCHAR(100),
    TinhTrang NVARCHAR(50),
    HinhAnh NVARCHAR(200),
    ViTri NVARCHAR(100)
)

------------------------------------------------
-- PHIẾU ĐẶT TOUR
------------------------------------------------
CREATE TABLE PhieuDatTour
(
    sophieudattour INT IDENTITY(1,1) PRIMARY KEY,
    makh INT,
    matour INT,
    loaitour NVARCHAR(50),
    ngaykhoihanh DATETIME,
    soluong INT,
    trangthai NVARCHAR(50),
    NgayDatTour DATETIME,
    NguoiXuLy NVARCHAR(100) NULL,

    FOREIGN KEY (makh)
    REFERENCES KhachHangs(MaKH)
    ON DELETE CASCADE,

    FOREIGN KEY (matour)
    REFERENCES TourDuLiches(MaTour)
)

------------------------------------------------
-- DỮ LIỆU TOUR
------------------------------------------------
INSERT INTO TourDuLiches
(TenTour, GiaVe, ThoiGianDi, TinhTrang, HinhAnh, ViTri)
VALUES
(N'Tour Nhật Bản Tokyo',18000000,N'5 ngày 4 đêm',N'Còn chỗ','tokyo.jpg',N'Nhật Bản'),
(N'Tour Nhật Bản Osaka',18500000,N'5 ngày 4 đêm',N'Còn chỗ','osaka.jpg',N'Nhật Bản'),
(N'Tour Nhật Bản Kyoto',17500000,N'5 ngày 4 đêm',N'Còn chỗ','kyoto.jpg',N'Nhật Bản'),
(N'Tour Hàn Quốc Seoul',17000000,N'5 ngày 4 đêm',N'Còn chỗ','seoul.jpg',N'Hàn Quốc'),
(N'Tour Hàn Quốc Busan',16500000,N'5 ngày 4 đêm',N'Còn chỗ','busan.jpg',N'Hàn Quốc'),
(N'Tour Singapore',12000000,N'4 ngày 3 đêm',N'Còn chỗ','singapore.jpg',N'Singapore'),
(N'Tour Bali Indonesia',14000000,N'5 ngày 4 đêm',N'Còn chỗ','bali.jpg',N'Indonesia'),
(N'Tour Phú Quốc',4500000,N'3 ngày 2 đêm',N'Còn chỗ','phuquoc.jpg',N'Việt Nam'),
(N'Tour Đà Nẵng',3500000,N'3 ngày 2 đêm',N'Còn chỗ','danang.jpg',N'Việt Nam'),
(N'Tour Hạ Long',2500000,N'2 ngày 1 đêm',N'Còn chỗ','halong.jpg',N'Việt Nam')

------------------------------------------------
-- ACCOUNT MẪU
------------------------------------------------
INSERT INTO Accounts (Username, Password, Phone, VaiTro)
VALUES
('admin','123','0900000000','Admin'),
('quanly1','123456','0900000001','QuanLy'),
('nhanvien1','123456','0900000002','NhanVien'),
('khach1','123456','0900000003','KhachHang')

------------------------------------------------
-- XEM DỮ LIỆU
------------------------------------------------
SELECT * FROM Accounts
SELECT * FROM KhachHangs
SELECT * FROM NhanViens
SELECT * FROM TourDuLiches
SELECT * FROM PhieuDatTour

