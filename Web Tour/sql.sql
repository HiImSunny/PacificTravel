ALTER TABLE DAT_TOUR
ADD ID_LOAI INT NOT NULL;

ALTER TABLE DAT_TOUR
ADD CONSTRAINT FK_DAT_TOUR_LOAI_TOUR
FOREIGN KEY (ID_LOAI) REFERENCES LOAI_TOUR(ID_LOAI);

DBCC CHECKIDENT ('[dbo].[LOAI_TOUR]', RESEED, 2);

DBCC CHECKIDENT ('[dbo].[TRANG_THAI]', RESEED, 0);

DBCC CHECKIDENT ('[dbo].[THANH_TOAN]', RESEED, 0);

DBCC CHECKIDENT ('[dbo].[DAT_TOUR]', RESEED, 0);

SELECT IDENT_CURRENT('TOUR')

DBCC CHECKIDENT ('[dbo].[TOUR]', RESEED, 1);

ALTER TABLE TOUR
ADD STT INT IDENTITY(1,1);

CREATE TABLE [dbo].[TRANG_THAI_THANH_TOAN] (
    [ID_TRANG_THAI] INT PRIMARY KEY IDENTITY(1, 1),
    [TEN_TRANG_THAI] NVARCHAR(20) NOT NULL
);

INSERT INTO [dbo].[TRANG_THAI_THANH_TOAN] (TEN_TRANG_THAI)
VALUES (N'Đã Thanh Toán'), (N'Chưa Thanh Toán');

EXEC sp_rename 'TRANG_THAI', 'TRANG_THAI_TOUR';

CREATE TABLE [dbo].[TRANG_THAI_TOUR] (
    [ID_TRANG_THAI]  INT           IDENTITY (1, 1) NOT NULL,
    [TEN_TRANG_THAI] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID_TRANG_THAI] ASC)
);

ALTER TABLE [dbo].[THANH_TOAN]
ADD [ID_TRANG_THAI] INT NOT NULL DEFAULT (1);

ALTER TABLE [dbo].[THANH_TOAN]
ADD CONSTRAINT FK_THANH_TOAN_TRANG_THAI
FOREIGN KEY (ID_TRANG_THAI) REFERENCES TRANG_THAI_THANH_TOAN(ID_TRANG_THAI);

ALTER TABLE DAT_TOUR
ADD HUY_DAT_TOUR BIT NOT NULL DEFAULT 0;

CREATE TABLE DIA_DIEM (
    ID_DIA_DIEM INT PRIMARY KEY IDENTITY(1,1),
    TEN_DIA_DIEM NVARCHAR(100) NOT NULL
);

INSERT INTO DIA_DIEM (TEN_DIA_DIEM)
VALUES ('An Giang'),
    ('Bà Rịa - Vũng Tàu'),
    ('Bắc Giang'),
    ('Bắc Kạn'),
    ('Bạc Liêu'),
    ('Bắc Ninh'),
    ('Bến Tre'),
    ('Bình Định'),
    ('Bình Dương'),
    ('Bình Phước'),
    ('Bình Thuận'),
    ('Cà Mau'),
    ('Cần Thơ'),
    ('Cao Bằng'),
    ('Đà Nẵng'),
    ('Đắk Lắk'),
    ('Đắk Nông'),
    ('Điện Biên'),
    ('Đồng Nai'),
    ('Đồng Tháp'),
    ('Gia Lai'),
    ('Hà Giang'),
    ('Hà Nam'),
    ('Hà Nội'),
    ('Hà Tĩnh'),
    ('Hải Dương'),
    ('Hải Phòng'),
    ('Hậu Giang'),
    ('Hòa Bình'),
    ('Hưng Yên'),
    ('Khánh Hòa'),
    ('Kiên Giang'),
    ('Kon Tum'),
    ('Lai Châu'),
    ('Lâm Đồng'),
    ('Lạng Sơn'),
    ('Lào Cai'),
    ('Long An'),
    ('Nam Định'),
    ('Nghệ An'),
    ('Ninh Bình'),
    ('Ninh Thuận'),
    ('Phú Thọ'),
    ('Phú Yên'),
    ('Quảng Bình'),
    ('Quảng Nam'),
    ('Quảng Ngãi'),
    ('Quảng Ninh'),
    ('Quảng Trị'),
    ('Sóc Trăng'),
    ('Sơn La'),
    ('Tây Ninh'),
    ('Thái Bình'),
    ('Thái Nguyên'),
    ('Thanh Hóa'),
    ('Thừa Thiên Huế'),
    ('Tiền Giang'),
    ('TP. Hồ Chí Minh'),
    ('Trà Vinh'),
    ('Tuyên Quang'),
    ('Vĩnh Long'),
    ('Vĩnh Phúc'),
    ('Yên Bái'), ('Bà Rịa - Vũng Tàu');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Nguyễn Văn A', 'nguyenvana@gmail.com', '0901234567', N'123 Đường Lý Thái Tổ, Quận 10, TP.HCM', 'MatKhau1!');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Trần Thị B', 'tranthib@gmail.com', '0912345678', N'456 Đường Trần Hưng Đạo, Quận 1, TP.HCM', 'MatKhau2@');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Lê Văn C', 'levanc@yahoo.com', '0923456789', N'789 Đường Nguyễn Huệ, Quận 3, TP.HCM', 'MatKhau3#');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Phạm Thị D', 'phamthid@hotmail.com', '0934567890', N'321 Đường Lê Lợi, Quận 5, TP.HCM', 'MatKhau4$');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Đỗ Minh E', 'dominhe@outlook.com', '0945678901', N'654 Đường Pasteur, Quận 1, TP.HCM', 'MatKhau5%');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Võ Thị F', 'vothif@gmail.com', '0956789012', N'987 Đường Phạm Ngũ Lão, Quận 6, TP.HCM', 'MatKhau6^');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Ngô Văn G', 'ngovang@gmail.com', '0967890123', N'123 Đường Bạch Đằng, Quận Bình Thạnh, TP.HCM', 'MatKhau7&');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Phan Thị H', 'phanthih@yahoo.com', '0978901234', N'456 Đường Võ Văn Tần, Quận 3, TP.HCM', 'MatKhau8*');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Hoàng Minh I', 'hoangminh@gmail.com', '0989012345', N'789 Đường Nguyễn Trãi, Quận 5, TP.HCM', 'MatKhau9(');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Bùi Thị J', 'buithij@hotmail.com', '0990123456', N'321 Đường Lý Tự Trọng, Quận 1, TP.HCM', 'MatKhau10)');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Nguyễn Văn K', 'nguyenvank@gmail.com', '0913456789', N'12 Đường Hai Bà Trưng, Quận 1, TP.HCM', 'MatKhau11!');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Trần Thị L', 'tranthil@gmail.com', '0924567890', N'34 Đường Nguyễn Thị Minh Khai, Quận 3, TP.HCM', 'MatKhau12@');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Lê Văn M', 'levanm@yahoo.com', '0935678901', N'56 Đường Nguyễn Văn Cừ, Quận 5, TP.HCM', 'MatKhau13#');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Phạm Thị N', 'phamthin@gmail.com', '0946789012', N'78 Đường Cộng Hòa, Quận Tân Bình, TP.HCM', 'MatKhau14$');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Đỗ Minh O', 'dominho@outlook.com', '0957890123', N'90 Đường Trường Sa, Quận Phú Nhuận, TP.HCM', 'MatKhau15%');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Võ Thị P', 'vothip@gmail.com', '0968901234', N'12 Đường Hoàng Sa, Quận Tân Phú, TP.HCM', 'MatKhau16^');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Ngô Văn Q', 'ngovanq@gmail.com', '0979012345', N'34 Đường Lê Quý Đôn, Quận 3, TP.HCM', 'MatKhau17&');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Phan Thị R', 'phanthir@yahoo.com', '0980123456', N'56 Đường Nam Kỳ Khởi Nghĩa, Quận 1, TP.HCM', 'MatKhau18*');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Hoàng Minh S', 'hoangminhs@gmail.com', '0991234567', N'78 Đường Lê Văn Sỹ, Quận Phú Nhuận, TP.HCM', 'MatKhau19(');

INSERT INTO THANH_VIEN (TEN_THANH_VIEN, EMAIL_THANH_VIEN, SDT_THANH_VIEN, DIA_CHI_THANH_VIEN, MAT_KHAU) 
VALUES (N'Bùi Thị T', 'buithit@hotmail.com', '0902345678', N'90 Đường Trần Hưng Đạo, Quận 5, TP.HCM', 'MatKhau20)');
