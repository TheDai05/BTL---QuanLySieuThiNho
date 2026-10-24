using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL___QuanLySieuThiNho.Models
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        [StringLength(10)]
        [Column("Ma_HD")]
        [Display(Name = "Mã hóa đơn")]
        public string MaHd { get; set; } = string.Empty;

        [Column("Ngay_lap")]
        [Display(Name = "Ngày lập")]
        public DateTime NgayLap { get; set; } = DateTime.Now;

        [StringLength(10)]
        [Column("Ma_NV")]
        public string? MaNv { get; set; }
        public NhanVien? NhanVien { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Ma_KH")]
        public string MaKh { get; set; } = string.Empty;
        public KhachHang? KhachHang { get; set; }

        [Column("Tong_tien", TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        [Column("Trang_thai")]
        [Display(Name = "Trạng thái")]
        public int? TrangThai { get; set; } = 0; // 0: Chờ xác nhận, 1: Đã xác nhận, 2: Đang giao, 3: Đã giao, 4: Đã hủy

        [StringLength(255)]
        [Column("Dia_chi_giao")]
        [Display(Name = "Địa chỉ giao")]
        public string? DiaChiGiao { get; set; }

        [StringLength(20)]
        [Column("Sdt_nhan")]
        [Display(Name = "Số điện thoại nhận")]
        public string? SdtNhan { get; set; }

        [StringLength(50)]
        [Column("Ten_nguoi_nhan")]
        [Display(Name = "Người nhận")]
        public string? TenNguoiNhan { get; set; }

        public ICollection<ChiTietHoaDon>? ChiTietHoaDons { get; set; }
    }

    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Column("Ma_HD")]
        [StringLength(10)]
        public string MaHd { get; set; } = string.Empty;
        public HoaDon? HoaDon { get; set; }

        [Column("Ma_SP")]
        [StringLength(10)]
        public string MaSp { get; set; } = string.Empty;
        public SanPham? SanPham { get; set; }

        [Column("So_luong")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Column("Don_gia", TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Column("Thanh_tien", TypeName = "decimal(18,2)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Thành tiền")]
        public decimal? ThanhTien { get; set; }
    }
}
