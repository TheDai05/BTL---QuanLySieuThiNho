using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL___QuanLySieuThiNho.Models
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        [StringLength(10)]
        [Column("Ma_SP")]
        [Display(Name = "Mã sản phẩm")]
        public string MaSp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(50)]
        [Column("Ten_SP")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập loại sản phẩm")]
        [StringLength(50)]
        [Column("Loai_SP")]
        [Display(Name = "Loại sản phẩm")]
        public string LoaiSp { get; set; } = string.Empty;

        [Column("So_luong_ton")]
        [Display(Name = "Tồn kho")]
        public int? SoLuongTon { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Column("Gia_ban", TypeName = "decimal(18,2)")]
        [Display(Name = "Giá bán")]
        public decimal GiaBan { get; set; }

        [StringLength(10)]
        [Column("Ma_NCC")]
        [Display(Name = "Mã NCC")]
        public string? MaNcc { get; set; }

        [Column("Hinh_anh")]
        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Column("Trang_thai")]
        [Display(Name = "Trạng thái")]
        public int? TrangThai { get; set; } = 1;

        // Navigation
        public ICollection<GioHang>? GioHangs { get; set; }
        public ICollection<ChiTietHoaDon>? ChiTietHoaDons { get; set; }
    }
}
