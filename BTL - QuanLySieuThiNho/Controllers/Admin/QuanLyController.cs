using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers.Admin
{
    public class QuanLyDonHangController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        public QuanLyDonHangController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(int? trangThai, int trang = 1)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            int soLuongMoiTrang = 10;
            var query = _context.HoaDons
                .Include(d => d.KhachHang)
                .AsQueryable();

            if (trangThai.HasValue)
                query = query.Where(d => d.TrangThai == trangThai.Value);

            query = query.OrderByDescending(d => d.NgayLap);

            int tongSo = await query.CountAsync();
            int tongSoTrang = (int)Math.Ceiling(tongSo / (double)soLuongMoiTrang);
            trang = Math.Max(1, Math.Min(trang, Math.Max(1, tongSoTrang)));

            var hoaDons = await query
                .Skip((trang - 1) * soLuongMoiTrang)
                .Take(soLuongMoiTrang)
                .ToListAsync();

            ViewBag.TrangThai = trangThai;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;

            return View(hoaDons);
        }

        public async Task<IActionResult> ChiTiet(string id)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            var donHang = await _context.HoaDons
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietHoaDons)!
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.MaHd == id);

            if (donHang == null) return NotFound();
            return View(donHang);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(string id, int trangThai)
        {
            if (!IsAdminLoggedIn()) return Json(new { success = false });

            var donHang = await _context.HoaDons
                .Include(d => d.ChiTietHoaDons)!
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.MaHd == id);

            if (donHang == null)
                return Json(new { success = false, message = "Không tìm thấy hóa đơn" });

            // Nếu đổi sang trạng thái hủy và trước đó chưa hủy -> hoàn kho
            if (trangThai == 4 && donHang.TrangThai != 4)
            {
                if (donHang.ChiTietHoaDons != null)
                {
                    foreach (var ct in donHang.ChiTietHoaDons)
                        if (ct.SanPham != null && ct.SanPham.SoLuongTon.HasValue)
                            ct.SanPham.SoLuongTon += ct.SoLuong;
                }
            }

            donHang.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            return Json(new { success = true, tenTrangThai = GetTenTrangThai(trangThai) });
        }

        private static string GetTenTrangThai(int tt) => tt switch
        {
            0 => "Chờ xác nhận",
            1 => "Đã xác nhận",
            2 => "Đang giao",
            3 => "Đã giao",
            4 => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public class QuanLyKhachHangController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        public QuanLyKhachHangController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? timKiem, int trang = 1)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            int soLuongMoiTrang = 10;
            var query = _context.KhachHangs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timKiem))
                query = query.Where(k => k.TenKh.Contains(timKiem) || k.SoDienThoai.Contains(timKiem) || (k.Email != null && k.Email.Contains(timKiem)));

            query = query.OrderBy(k => k.MaKh);

            int tongSo = await query.CountAsync();
            int tongSoTrang = (int)Math.Ceiling(tongSo / (double)soLuongMoiTrang);
            trang = Math.Max(1, Math.Min(trang, Math.Max(1, tongSoTrang)));

            var khachHangs = await query
                .Skip((trang - 1) * soLuongMoiTrang)
                .Take(soLuongMoiTrang)
                .ToListAsync();

            ViewBag.TimKiem = timKiem;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;

            return View(khachHangs);
        }

        public async Task<IActionResult> ChiTiet(string id)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            var kh = await _context.KhachHangs
                .Include(k => k.HoaDons)
                .FirstOrDefaultAsync(k => k.MaKh == id);

            if (kh == null) return NotFound();
            return View(kh);
        }
    }
}
