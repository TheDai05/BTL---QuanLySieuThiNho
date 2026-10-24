using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using BTL___QuanLySieuThiNho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index(string? timKiem, string? loaiSp, string sapXep = "default", int trang = 1)
        {
            int soLuongMoiTrang = 8;

            var query = _context.SanPhams
                .Where(s => s.TrangThai == null || s.TrangThai == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(loaiSp))
                query = query.Where(s => s.LoaiSp == loaiSp);

            if (!string.IsNullOrWhiteSpace(timKiem))
                query = query.Where(s => s.TenSp.Contains(timKiem));

            query = sapXep switch
            {
                "giatang" => query.OrderBy(s => s.GiaBan),
                "giagiam" => query.OrderByDescending(s => s.GiaBan),
                "moi" => query.OrderByDescending(s => s.MaSp),
                _ => query.OrderBy(s => s.TenSp)
            };

            int tongSo = await query.CountAsync();
            int tongSoTrang = (int)Math.Ceiling(tongSo / (double)soLuongMoiTrang);
            trang = Math.Max(1, Math.Min(trang, Math.Max(1, tongSoTrang)));

            var sanPhams = await query
                .Skip((trang - 1) * soLuongMoiTrang)
                .Take(soLuongMoiTrang)
                .ToListAsync();

            var loaiList = await _context.SanPhams
                .Select(s => s.LoaiSp)
                .Distinct()
                .ToListAsync();

            var vm = new SanPhamListVM
            {
                SanPhams = sanPhams,
                LoaiSanPhams = loaiList,
                TimKiem = timKiem,
                LoaiSp = loaiSp,
                SapXep = sapXep,
                TrangHienTai = trang,
                TongSoTrang = tongSoTrang,
                SoLuongMoiTrang = soLuongMoiTrang
            };

            return View(vm);
        }

        // GET: /Home/ChiTiet/{id}
        public async Task<IActionResult> ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sanPham = await _context.SanPhams
                .FirstOrDefaultAsync(s => s.MaSp == id);

            if (sanPham == null) return NotFound();

            ViewBag.SanPhamLienQuan = await _context.SanPhams
                .Where(s => s.LoaiSp == sanPham.LoaiSp && s.MaSp != id && (s.TrangThai == null || s.TrangThai == 1))
                .Take(4)
                .ToListAsync();

            return View(sanPham);
        }

        [HttpGet]
        public async Task<IActionResult> TimKiemGoiY(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());

            var results = await _context.SanPhams
                .Where(s => (s.TrangThai == null || s.TrangThai == 1) && s.TenSp.Contains(q))
                .Take(5)
                .Select(s => new { s.MaSp, TenSanPham = s.TenSp, Gia = s.GiaBan, HinhAnh = s.HinhAnh })
                .ToListAsync();

            return Json(results);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
