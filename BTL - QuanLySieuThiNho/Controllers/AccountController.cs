using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using BTL___QuanLySieuThiNho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetString("KhachHangMa") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.KhachHangs.AnyAsync(k => k.TenDangNhap == model.TenDangNhap || k.SoDienThoai == model.SoDienThoai))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc Số điện thoại đã được đăng ký");
                return View(model);
            }

            // Sinh mã KH tự động
            int count = await _context.KhachHangs.CountAsync() + 1;
            string maKh = "KH" + count.ToString("D3");
            while (await _context.KhachHangs.AnyAsync(k => k.MaKh == maKh))
            {
                count++;
                maKh = "KH" + count.ToString("D3");
            }

            var khachHang = new KhachHang
            {
                MaKh = maKh,
                TenKh = model.HoTen,
                TenDangNhap = model.TenDangNhap,
                Email = model.Email,
                MatKhau = model.MatKhau,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi,
                DiemTichLuy = 0
            };

            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(DangNhap));
        }

        [HttpGet]
        public IActionResult DangNhap(string? returnUrl)
        {
            if (HttpContext.Session.GetString("KhachHangMa") != null)
                return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapVM model, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            string username = (model.TenDangNhap ?? "").Trim();
            string password = (model.MatKhau ?? "").Trim();

            // 1. Kiểm tra tài khoản Khách Hàng
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => (k.TenDangNhap == username || k.SoDienThoai == username || k.MaKh == username) 
                                       && k.MatKhau == password);

            if (khachHang != null)
            {
                HttpContext.Session.SetString("KhachHangMa", khachHang.MaKh);
                HttpContext.Session.SetString("KhachHangHoTen", khachHang.TenKh);

                if (model.GhiNho)
                {
                    Response.Cookies.Append("KhachHangMa", khachHang.MaKh, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(30),
                        HttpOnly = true
                    });
                }

                TempData["Success"] = $"Chào mừng {khachHang.TenKh}!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            // 2. Tiện ích: Nếu người dùng nhập tài khoản Quản trị viên/Nhân viên ở form này
            var admin = await _context.NhanViens
                .FirstOrDefaultAsync(q => (q.TenDangNhap == username || q.MaNv == username) 
                                       && q.MatKhau == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("AdminMaNv", admin.MaNv);
                HttpContext.Session.SetString("AdminHoTen", admin.TenNv);
                TempData["Success"] = $"Chào mừng Quản trị viên {admin.TenNv}!";
                return Redirect("/Admin/Dashboard");
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
            return View(model);
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("KhachHangMa");
            TempData["Success"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ThongTin()
        {
            var maKh = HttpContext.Session.GetString("KhachHangMa");
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction(nameof(DangNhap));

            var khachHang = await _context.KhachHangs.FindAsync(maKh);
            if (khachHang == null) return NotFound();

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTin(KhachHang model)
        {
            var maKh = HttpContext.Session.GetString("KhachHangMa");
            if (string.IsNullOrEmpty(maKh)) return RedirectToAction(nameof(DangNhap));

            var khachHang = await _context.KhachHangs.FindAsync(maKh);
            if (khachHang == null) return NotFound();

            khachHang.TenKh = model.TenKh;
            khachHang.SoDienThoai = model.SoDienThoai;
            khachHang.DiaChi = model.DiaChi;
            khachHang.Email = model.Email;

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("KhachHangHoTen", khachHang.TenKh);

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction(nameof(ThongTin));
        }
    }
}
