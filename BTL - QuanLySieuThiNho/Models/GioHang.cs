using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL___QuanLySieuThiNho.Models
{
    [Table("GioHang")]
    public class GioHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Ma_KH")]
        public string MaKh { get; set; } = string.Empty;
        public KhachHang? KhachHang { get; set; }

        [Required]
        [StringLength(10)]
        [Column("Ma_SP")]
        public string MaSp { get; set; } = string.Empty;
        public SanPham? SanPham { get; set; }

        [Column("So_luong")]
        public int SoLuong { get; set; } = 1;

        [Column("Ngay_tao")]
        public DateTime? NgayTao { get; set; } = DateTime.Now;
    }
}
