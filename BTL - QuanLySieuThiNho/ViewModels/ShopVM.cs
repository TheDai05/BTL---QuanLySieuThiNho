using BTL___QuanLySieuThiNho.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BTL___QuanLySieuThiNho.ViewModels
{
    public class SanPhamListVM
    {
        public IEnumerable<SanPham> SanPhams { get; set; } = new List<SanPham>();
        public IEnumerable<string> LoaiSanPhams { get; set; } = new List<string>();
        public string? TimKiem { get; set; }
        public string? LoaiSp { get; set; }
        public string SapXep { get; set; } = "default";

        // Pagination
        public int TrangHienTai { get; set; } = 1;
        public int TongSoTrang { get; set; }
        public int SoLuongMoiTrang { get; set; } = 8;
    }

    public class GioHangVM
    {
        public List<GioHangItemVM> Items { get; set; } = new();
        public decimal TongTien => Items.Sum(x => x.ThanhTien);
        public int TongSoLuong => Items.Sum(x => x.SoLuong);
    }

    public class GioHangItemVM
    {
        public int GioHangId { get; set; }
        public string MaSp { get; set; } = string.Empty;
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }

    public class DatHangVM
    {
        [ValidateNever]
        public GioHangVM GioHang { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [Display(Name = "Họ tên người nhận")]
        public string HoTenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^[0-9]{9,12}$", ErrorMessage = "Số điện thoại không hợp lệ (từ 9 đến 12 chữ số)")]
        [Display(Name = "Số điện thoại")]
        public string SdtNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChiGiao { get; set; } = string.Empty;
    }
}
