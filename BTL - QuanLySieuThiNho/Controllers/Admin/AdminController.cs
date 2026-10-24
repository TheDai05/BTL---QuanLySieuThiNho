using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers.Admin
{
    public class AdminBaseController : Controller
    {
        protected bool IsAdminLoggedIn() =>
            HttpContext.Session.GetString("AdminMaNv") != null;

        protected IActionResult RequireAdminLogin()
        {
            TempData["Error"] = "Vui lòng đăng nhập quản trị!";
            return RedirectToAction("DangNhap", "AdminAccount");
        }
    }

    public class AdminAccountController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        public AdminAccountController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult DangNhap()
        {
            if (IsAdminLoggedIn()) return Redirect("/Admin/Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(AdminDangNhapVM model)
        {
            if (!ModelState.IsValid) return View(model);

            string username = (model.TenDangNhap ?? "").Trim();
            string password = (model.MatKhau ?? "").Trim();

            var admin = await _context.NhanViens
                .FirstOrDefaultAsync(q => (q.TenDangNhap == username || q.MaNv == username) 
                                       && q.MatKhau == password);

            if (admin == null)
            {
                var kh = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => (k.TenDangNhap == username || k.SoDienThoai == username || k.MaKh == username) 
                                           && k.MatKhau == password);
                if (kh != null)
                {
                    ModelState.AddModelError("", "Đây là tài khoản Khách Hàng. Vui lòng đăng nhập tại trang Khách hàng (trên thanh Menu).");
                    return View(model);
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu quản trị không đúng");
                return View(model);
            }

            HttpContext.Session.SetString("AdminMaNv", admin.MaNv);
            HttpContext.Session.SetString("AdminHoTen", admin.TenNv);
            return Redirect("/Admin/Dashboard");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Remove("AdminMaNv");
            HttpContext.Session.Remove("AdminHoTen");
            return Redirect("/Admin/AdminAccount/DangNhap");
        }
    }

    public class DashboardController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            ViewBag.TongSanPham = await _context.SanPhams.CountAsync();
            ViewBag.TongKhachHang = await _context.KhachHangs.CountAsync();
            ViewBag.TongDonHang = await _context.HoaDons.CountAsync();
            ViewBag.DoanhThu = await _context.HoaDons
                .Where(d => d.TrangThai == 3 || d.TrangThai == 1) // Đã giao hoặc đã xác nhận
                .SumAsync(d => (decimal?)d.TongTien) ?? 0;
            ViewBag.DonHangChoXacNhan = await _context.HoaDons
                .CountAsync(d => d.TrangThai == 0);
            ViewBag.DonHangGanDay = await _context.HoaDons
                .Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayLap)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
