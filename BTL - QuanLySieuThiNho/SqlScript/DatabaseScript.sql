IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [DanhMucs] (
    [Id] int NOT NULL IDENTITY,
    [TenDanhMuc] nvarchar(max) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [HinhAnh] nvarchar(max) NULL,
    CONSTRAINT [PK_DanhMucs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KhachHangs] (
    [Id] int NOT NULL IDENTITY,
    [HoTen] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [MatKhau] nvarchar(max) NOT NULL,
    [DiaChi] nvarchar(max) NULL,
    [SoDienThoai] nvarchar(max) NULL,
    [NgayTao] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_KhachHangs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [QuanLys] (
    [Id] int NOT NULL IDENTITY,
    [TenDangNhap] nvarchar(max) NOT NULL,
    [MatKhau] nvarchar(max) NOT NULL,
    [HoTen] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_QuanLys] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SanPhams] (
    [Id] int NOT NULL IDENTITY,
    [TenSanPham] nvarchar(max) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [Gia] decimal(18,2) NOT NULL,
    [HinhAnh] nvarchar(max) NULL,
    [SoLuong] int NOT NULL,
    [NgayThem] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [DanhMucId] int NOT NULL,
    CONSTRAINT [PK_SanPhams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SanPhams_DanhMucs_DanhMucId] FOREIGN KEY ([DanhMucId]) REFERENCES [DanhMucs] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DonHangs] (
    [Id] int NOT NULL IDENTITY,
    [KhachHangId] int NOT NULL,
    [NgayDat] datetime2 NOT NULL,
    [TongTien] decimal(18,2) NOT NULL,
    [TrangThai] int NOT NULL,
    [GhiChu] nvarchar(max) NULL,
    [HoTenNguoiNhan] nvarchar(max) NOT NULL,
    [SdtNguoiNhan] nvarchar(max) NOT NULL,
    [DiaChiGiao] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_DonHangs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DonHangs_KhachHangs_KhachHangId] FOREIGN KEY ([KhachHangId]) REFERENCES [KhachHangs] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [GioHangs] (
    [Id] int NOT NULL IDENTITY,
    [KhachHangId] int NOT NULL,
    [SanPhamId] int NOT NULL,
    [SoLuong] int NOT NULL,
    [NgayThem] datetime2 NOT NULL,
    CONSTRAINT [PK_GioHangs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GioHangs_KhachHangs_KhachHangId] FOREIGN KEY ([KhachHangId]) REFERENCES [KhachHangs] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GioHangs_SanPhams_SanPhamId] FOREIGN KEY ([SanPhamId]) REFERENCES [SanPhams] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DonHangChiTiets] (
    [Id] int NOT NULL IDENTITY,
    [DonHangId] int NOT NULL,
    [SanPhamId] int NOT NULL,
    [SoLuong] int NOT NULL,
    [DonGia] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_DonHangChiTiets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DonHangChiTiets_DonHangs_DonHangId] FOREIGN KEY ([DonHangId]) REFERENCES [DonHangs] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DonHangChiTiets_SanPhams_SanPhamId] FOREIGN KEY ([SanPhamId]) REFERENCES [SanPhams] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'HinhAnh', N'MoTa', N'TenDanhMuc') AND [object_id] = OBJECT_ID(N'[DanhMucs]'))
    SET IDENTITY_INSERT [DanhMucs] ON;
INSERT INTO [DanhMucs] ([Id], [HinhAnh], [MoTa], [TenDanhMuc])
VALUES (1, NULL, N'Các mặt hàng thực phẩm tươi sống và đóng gói', N'Thực phẩm'),
(2, NULL, N'Nước giải khát, nước ngọt, bia, rượu', N'Đồ uống'),
(3, NULL, N'Sản phẩm tẩy rửa, vệ sinh nhà cửa', N'Hóa phẩm'),
(4, NULL, N'Các loại bánh, kẹo, snack', N'Bánh kẹo'),
(5, NULL, N'Muối, đường, bột ngọt, nước mắm, dầu ăn', N'Gia vị');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'HinhAnh', N'MoTa', N'TenDanhMuc') AND [object_id] = OBJECT_ID(N'[DanhMucs]'))
    SET IDENTITY_INSERT [DanhMucs] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'HoTen', N'MatKhau', N'TenDangNhap') AND [object_id] = OBJECT_ID(N'[QuanLys]'))
    SET IDENTITY_INSERT [QuanLys] ON;
INSERT INTO [QuanLys] ([Id], [HoTen], [MatKhau], [TenDangNhap])
VALUES (1, N'Quản trị viên', N'admin123', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'HoTen', N'MatKhau', N'TenDangNhap') AND [object_id] = OBJECT_ID(N'[QuanLys]'))
    SET IDENTITY_INSERT [QuanLys] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DanhMucId', N'Gia', N'HinhAnh', N'IsActive', N'MoTa', N'NgayThem', N'SoLuong', N'TenSanPham') AND [object_id] = OBJECT_ID(N'[SanPhams]'))
    SET IDENTITY_INSERT [SanPhams] ON;
INSERT INTO [SanPhams] ([Id], [DanhMucId], [Gia], [HinhAnh], [IsActive], [MoTa], [NgayThem], [SoLuong], [TenSanPham])
VALUES (1, 1, 85000.0, NULL, CAST(1 AS bit), N'Gạo thơm dẻo loại 1', '2026-10-22T10:42:29.8016780+07:00', 100, N'Gạo ST25 (5kg)'),
(2, 1, 95000.0, NULL, CAST(1 AS bit), N'Mì ăn liền vị tôm chua cay', '2026-10-22T10:42:29.8016799+07:00', 50, N'Mì Hảo Hảo (thùng 30 gói)'),
(3, 2, 8000.0, NULL, CAST(1 AS bit), N'Nước tinh khiết đóng chai', '2026-10-22T10:42:29.8016800+07:00', 200, N'Nước suối Aquafina 500ml'),
(4, 2, 65000.0, NULL, CAST(1 AS bit), N'Nước ngọt có ga', '2026-10-22T10:42:29.8016802+07:00', 80, N'Coca Cola 330ml (lốc 6)'),
(5, 3, 32000.0, NULL, CAST(1 AS bit), N'Nước rửa chén hương chanh', '2026-10-22T10:42:29.8016804+07:00', 60, N'Nước rửa chén Sunlight 750ml'),
(6, 4, 28000.0, NULL, CAST(1 AS bit), N'Bánh quy nhân kem chocolate', '2026-10-22T10:42:29.8016805+07:00', 120, N'Bánh Oreo (hộp)'),
(7, 5, 45000.0, NULL, CAST(1 AS bit), N'Nước mắm truyền thống 40 độ đạm', '2026-10-22T10:42:29.8016806+07:00', 70, N'Nước mắm Phú Quốc 500ml'),
(8, 5, 55000.0, NULL, CAST(1 AS bit), N'Dầu thực vật tinh luyện', '2026-10-22T10:42:29.8016808+07:00', 90, N'Dầu ăn Neptune 1L');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DanhMucId', N'Gia', N'HinhAnh', N'IsActive', N'MoTa', N'NgayThem', N'SoLuong', N'TenSanPham') AND [object_id] = OBJECT_ID(N'[SanPhams]'))
    SET IDENTITY_INSERT [SanPhams] OFF;
GO

CREATE INDEX [IX_DonHangChiTiets_DonHangId] ON [DonHangChiTiets] ([DonHangId]);
GO

CREATE INDEX [IX_DonHangChiTiets_SanPhamId] ON [DonHangChiTiets] ([SanPhamId]);
GO

CREATE INDEX [IX_DonHangs_KhachHangId] ON [DonHangs] ([KhachHangId]);
GO

CREATE INDEX [IX_GioHangs_KhachHangId] ON [GioHangs] ([KhachHangId]);
GO

CREATE INDEX [IX_GioHangs_SanPhamId] ON [GioHangs] ([SanPhamId]);
GO

CREATE INDEX [IX_SanPhams_DanhMucId] ON [SanPhams] ([DanhMucId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261022034230_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

