using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using BTL___QuanLySieuThiNho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetKhachHangMa() => HttpContext.Session.GetString("KhachHangMa");

        public async Task<IActionResult> Index()
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction("DangNhap", "Account", new { returnUrl = "/GioHang" });

            var items = await _context.GioHangs
                .Include(g => g.SanPham)
                .Where(g => g.MaKh == maKh)
                .ToListAsync();

            var vm = new GioHangVM
            {
                Items = items.Select(g => new GioHangItemVM
                {
                    GioHangId = g.Id,
                    MaSp = g.MaSp,
                    TenSanPham = g.SanPham!.TenSp,
                    HinhAnh = g.SanPham.HinhAnh,
                    DonGia = g.SanPham.GiaBan,
                    SoLuong = g.SoLuong
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(string sanPhamId, int soLuong = 1)
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh))
                return Json(new { success = false, message = "Vui lòng đăng nhập để mua hàng" });

            var sanPham = await _context.SanPhams.FindAsync(sanPhamId);
            if (sanPham == null || (sanPham.TrangThai != null && sanPham.TrangThai != 1))
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc đã ngừng bán" });

            if (soLuong < 1) soLuong = 1;
            int tonKho = sanPham.SoLuongTon ?? 0;
            if (soLuong > tonKho)
                return Json(new { success = false, message = $"Chỉ còn {tonKho} sản phẩm trong kho" });

            var existing = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaKh == maKh && g.MaSp == sanPhamId);

            if (existing != null)
            {
                existing.SoLuong = Math.Min(existing.SoLuong + soLuong, tonKho);
            }
            else
            {
                _context.GioHangs.Add(new GioHang
                {
                    MaKh = maKh,
                    MaSp = sanPhamId,
                    SoLuong = soLuong,
                    NgayTao = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            int tongSoLuong = await _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .SumAsync(g => g.SoLuong);

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", soLuongGio = tongSoLuong });
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(int gioHangId, int soLuong)
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return Json(new { success = false });

            var item = await _context.GioHangs
                .Include(g => g.SanPham)
                .FirstOrDefaultAsync(g => g.Id == gioHangId && g.MaKh == maKh);

            if (item == null) return Json(new { success = false });

            int tonKho = item.SanPham?.SoLuongTon ?? 0;
            if (soLuong <= 0)
            {
                _context.GioHangs.Remove(item);
            }
            else
            {
                item.SoLuong = Math.Min(soLuong, tonKho > 0 ? tonKho : soLuong);
            }

            await _context.SaveChangesAsync();

            var items = await _context.GioHangs
                .Include(g => g.SanPham)
                .Where(g => g.MaKh == maKh)
                .ToListAsync();

            decimal tongTien = items.Sum(g => g.SanPham!.GiaBan * g.SoLuong);
            decimal thanhTien = soLuong > 0 ? item.SanPham!.GiaBan * item.SoLuong : 0;
            int tongSoLuong = items.Sum(g => g.SoLuong);

            return Json(new { success = true, thanhTien, tongTien, tongSoLuong });
        }

        [HttpPost]
        public async Task<IActionResult> XoaKhoiGio(int gioHangId)
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return Json(new { success = false });

            var item = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.Id == gioHangId && g.MaKh == maKh);

            if (item != null)
            {
                _context.GioHangs.Remove(item);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> SoLuongGio()
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return Json(new { count = 0 });

            int count = await _context.GioHangs
                .Where(g => g.MaKh == maKh)
                .SumAsync(g => (int?)g.SoLuong) ?? 0;

            return Json(new { count });
        }
    }
}
