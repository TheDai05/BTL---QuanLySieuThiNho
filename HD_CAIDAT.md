# HƯỚNG DẪN CÀI ĐẶT VÀ KHỞI CHẠY BÀI TẬP LỚN - QUẢN LÝ SIÊU THỊ NHỎ

## 1. Cấu hình Cơ sở dữ liệu
Hệ thống đã được kết nối trực tiếp vào cơ sở dữ liệu **`QuanLySieuThiNho`** có sẵn trên máy của bạn:
- **Chuỗi kết nối (appsettings.json)**:
  ```json
  "DefaultConnection": "Server=.;Database=QuanLySieuThiNho;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  ```
- **Các bảng thực tế đã ánh xạ**:
  - `SanPham`: Ánh xạ các trường `Ma_SP`, `Ten_SP`, `Loai_SP`, `Gia_ban`, `So_luong_ton`, `Hinh_anh`, `Trang_thai`.
  - `KhachHang`: Ánh xạ các trường `Ma_KH`, `Ten_KH`, `So_dien_thoai`, `Dia_chi`, `Diem_tich_luy`, `Ten_dang_nhap`, `Mat_khau`, `Email`.
  - `HoaDon` & `ChiTietHoaDon`: Quản lý đơn đặt hàng, tính tổng tiền, trạng thái giao nhận và người nhận hàng.
  - `NhanVien`: Quản lý tài khoản quản trị viên / nhân viên.
  - `GioHang`: Lưu giỏ hàng của từng khách hàng.

## 2. Thông tin tài khoản đăng nhập mẫu

### Tài khoản Khách hàng (Đã có sẵn trong CSDL):
- **Tên đăng nhập / SĐT**: `kh001` (hoặc `0968903705`)
- **Mật khẩu**: `123`
*(Bạn cũng có thể tự nhấn "Đăng ký" tài khoản mới bất cứ lúc nào trên web)*

### Tài khoản Quản trị viên (Admin - Đã có sẵn trong CSDL):
- **Đường dẫn**: `/Admin/AdminAccount/DangNhap`
- **Tên đăng nhập**: `daiql001@admin` (hoặc `QL001`)
- **Mật khẩu**: `123`
- **Họ tên**: Nguyễn Thế Đại (Quản lý)

## 3. Cách chạy ứng dụng
1. Mở thư mục `BTL - QuanLySieuThiNho` trong Visual Studio hoặc VS Code.
2. Nhấn **F5** hoặc gõ lệnh sau trên terminal:
   ```bash
   dotnet run
   ```
3. Truy cập trình duyệt theo URL hiển thị (thường là `https://localhost:7xxx` hoặc `http://localhost:5xxx`).
