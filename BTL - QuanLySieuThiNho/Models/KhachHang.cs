using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL___QuanLySieuThiNho.Models
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [StringLength(10)]
        [Column("Ma_KH")]
        public string MaKh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(50)]
        [Display(Name = "Họ tên")]
        [Column("Ten_KH")]
        public string TenKh { get; set; } = string.Empty;

        [Column("Ngay_sinh", TypeName = "date")]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Column("Gioi_tinh")]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15)]
        [Column("So_dien_thoai")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("Dia_chi")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Column("Diem_tich_luy")]
        [Display(Name = "Điểm tích lũy")]
        public int? DiemTichLuy { get; set; } = 0;

        [StringLength(50)]
        [Column("Ten_dang_nhap")]
        [Display(Name = "Tên đăng nhập")]
        public string? TenDangNhap { get; set; }

        [StringLength(100)]
        [Column("Mat_khau")]
        [Display(Name = "Mật khẩu")]
        public string? MatKhau { get; set; }

        [StringLength(100)]
        [Column("Email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        // Navigation
        public ICollection<HoaDon>? HoaDons { get; set; }
        public ICollection<GioHang>? GioHangs { get; set; }
    }
}
