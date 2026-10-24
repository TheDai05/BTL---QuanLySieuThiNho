using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using BTL___QuanLySieuThiNho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetKhachHangMa() => HttpContext.Session.GetString("KhachHangMa");

        public async Task<IActionResult> DatHang()
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction("DangNhap", "Account", new { returnUrl = "/DonHang/DatHang" });

            var gioHangItems = await _context.GioHangs
                .Include(g => g.SanPham)
                .Where(g => g.MaKh == maKh)
                .ToListAsync();

            if (!gioHangItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "GioHang");
            }

            var khachHang = await _context.KhachHangs.FindAsync(maKh);

            var vm = new DatHangVM
            {
                GioHang = new GioHangVM
                {
                    Items = gioHangItems.Select(g => new GioHangItemVM
                    {
                        GioHangId = g.Id,
                        MaSp = g.MaSp,
                        TenSanPham = g.SanPham!.TenSp,
                        HinhAnh = g.SanPham.HinhAnh,
                        DonGia = g.SanPham.GiaBan,
                        SoLuong = g.SoLuong
                    }).ToList()
                },
                HoTenNguoiNhan = khachHang?.TenKh ?? "",
                SdtNguoiNhan = khachHang?.SoDienThoai ?? "",
                DiaChiGiao = khachHang?.DiaChi ?? ""
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatHang(DatHangVM model)
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction("DangNhap", "Account");

            var gioHangItems = await _context.GioHangs
                .Include(g => g.SanPham)
                .Where(g => g.MaKh == maKh)
                .ToListAsync();

            if (!gioHangItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "GioHang");
            }

            model.GioHang = new GioHangVM
            {
                Items = gioHangItems.Select(g => new GioHangItemVM
                {
                    GioHangId = g.Id,
                    MaSp = g.MaSp,
                    TenSanPham = g.SanPham!.TenSp,
                    HinhAnh = g.SanPham.HinhAnh,
                    DonGia = g.SanPham.GiaBan,
                    SoLuong = g.SoLuong
                }).ToList()
            };

            if (!ModelState.IsValid) return View("DatHang", model);

            // Kiểm tra tồn kho
            foreach (var item in gioHangItems)
            {
                int tonKho = item.SanPham?.SoLuongTon ?? 0;
                if (item.SoLuong > tonKho)
                {
                    ModelState.AddModelError("", $"Sản phẩm '{item.SanPham?.TenSp}' chỉ còn {tonKho} sản phẩm");
                    return View("DatHang", model);
                }
            }

            // Sinh mã hóa đơn mới theo số lớn nhất hiện tại
            var existingIds = await _context.HoaDons
                .Where(h => h.MaHd.StartsWith("HD"))
                .Select(h => h.MaHd)
                .ToListAsync();
            int maxId = 0;
            foreach (var id in existingIds)
            {
                if (id.Length > 2 && int.TryParse(id.Substring(2), out int num) && num > maxId)
                {
                    maxId = num;
                }
            }
            string maHd = "HD" + (maxId + 1).ToString("D3");

            // Gán mã nhân viên bán hàng mặc định cho đơn online (NVBH001)
            string maNv = "NVBH001";

            var hoaDon = new HoaDon
            {
                MaHd = maHd,
                MaKh = maKh,
                MaNv = maNv,
                NgayLap = DateTime.Now,
                TongTien = gioHangItems.Sum(g => g.SanPham!.GiaBan * g.SoLuong),
                TrangThai = 0, // Chờ xác nhận
                TenNguoiNhan = model.HoTenNguoiNhan,
                SdtNhan = model.SdtNguoiNhan,
                DiaChiGiao = model.DiaChiGiao
            };

            _context.HoaDons.Add(hoaDon);

            // Thêm chi tiết hóa đơn & trừ tồn kho (Lưu ý: Thanh_tien là computed column trong SQL, không gán giá trị)
            foreach (var item in gioHangItems)
            {
                _context.ChiTietHoaDons.Add(new ChiTietHoaDon
                {
                    MaHd = maHd,
                    MaSp = item.MaSp,
                    SoLuong = item.SoLuong,
                    DonGia = item.SanPham!.GiaBan
                });

                if (item.SanPham.SoLuongTon.HasValue)
                {
                    item.SanPham.SoLuongTon = Math.Max(0, item.SanPham.SoLuongTon.Value - item.SoLuong);
                }
            }

            _context.GioHangs.RemoveRange(gioHangItems);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu đơn hàng: " + ex.GetBaseException().Message);
                return View("DatHang", model);
            }

            TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: {hoaDon.MaHd}";
            return RedirectToAction(nameof(LichSuMuaHang));
        }

        public async Task<IActionResult> LichSuMuaHang()
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction("DangNhap", "Account");

            var hoaDons = await _context.HoaDons
                .Include(d => d.ChiTietHoaDons)!
                    .ThenInclude(ct => ct.SanPham)
                .Where(d => d.MaKh == maKh)
                .OrderByDescending(d => d.NgayLap)
                .ToListAsync();

            return View(hoaDons);
        }

        public async Task<IActionResult> ChiTiet(string id)
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction("DangNhap", "Account");

            var hoaDon = await _context.HoaDons
                .Include(d => d.ChiTietHoaDons)!
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.MaHd == id && d.MaKh == maKh);

            if (hoaDon == null) return NotFound();

            return View(hoaDon);
        }

        [HttpPost]
        public async Task<IActionResult> HuyDon(string id)
        {
            var maKh = GetKhachHangMa();
            if (string.IsNullOrEmpty(maKh)) return Json(new { success = false });

            var donHang = await _context.HoaDons
                .Include(d => d.ChiTietHoaDons)!
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.MaHd == id && d.MaKh == maKh);

            if (donHang == null || donHang.TrangThai != 0)
                return Json(new { success = false, message = "Không thể hủy đơn hàng này" });

            donHang.TrangThai = 4; // Đã hủy

            // Hoàn lại tồn kho
            if (donHang.ChiTietHoaDons != null)
            {
                foreach (var ct in donHang.ChiTietHoaDons)
                {
                    if (ct.SanPham != null && ct.SanPham.SoLuongTon.HasValue)
                    {
                        ct.SanPham.SoLuongTon += ct.SoLuong;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
