using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL___QuanLySieuThiNho.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [StringLength(10)]
        [Column("Ma_NV")]
        public string MaNv { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("Ten_NV")]
        [Display(Name = "Họ tên")]
        public string TenNv { get; set; } = string.Empty;

        [Column("Ngay_sinh", TypeName = "date")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Column("Gioi_tinh")]
        public string? GioiTinh { get; set; }

        [StringLength(15)]
        [Column("So_dien_thoai")]
        public string? SoDienThoai { get; set; }

        [StringLength(255)]
        [Column("Dia_chi")]
        public string? DiaChi { get; set; }

        [StringLength(50)]
        [Column("Chuc_vu")]
        [Display(Name = "Chức vụ")]
        public string? ChucVu { get; set; }

        [Column("Luong", TypeName = "decimal(18,2)")]
        public decimal? Luong { get; set; }

        [Column("Hinh_anh")]
        public string? HinhAnh { get; set; }

        [Column("Trang_thai")]
        public bool? TrangThai { get; set; } = true;

        [StringLength(50)]
        [Column("Ten_dang_nhap")]
        [Display(Name = "Tên đăng nhập")]
        public string? TenDangNhap { get; set; }

        [StringLength(50)]
        [Column("Mat_khau")]
        [Display(Name = "Mật khẩu")]
        public string? MatKhau { get; set; }

        [StringLength(20)]
        [Column("Vai_tro")]
        [Display(Name = "Vai trò")]
        public string? VaiTro { get; set; }

        public ICollection<HoaDon>? HoaDons { get; set; }
    }
}
