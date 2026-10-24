using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers.Admin
{
    public class QuanLySanPhamController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public QuanLySanPhamController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(string? timKiem, string? loaiSp, int trang = 1)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            int soLuongMoiTrang = 10;
            var query = _context.SanPhams.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timKiem))
                query = query.Where(s => s.TenSp.Contains(timKiem) || s.MaSp.Contains(timKiem));

            if (!string.IsNullOrWhiteSpace(loaiSp))
                query = query.Where(s => s.LoaiSp == loaiSp);

            query = query.OrderBy(s => s.MaSp);

            int tongSo = await query.CountAsync();
            int tongSoTrang = (int)Math.Ceiling(tongSo / (double)soLuongMoiTrang);
            trang = Math.Max(1, Math.Min(trang, Math.Max(1, tongSoTrang)));

            var sanPhams = await query
                .Skip((trang - 1) * soLuongMoiTrang)
                .Take(soLuongMoiTrang)
                .ToListAsync();

            ViewBag.TimKiem = timKiem;
            ViewBag.LoaiSp = loaiSp;
            ViewBag.TrangHienTai = trang;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.Categories = await _context.SanPhams.Select(s => s.LoaiSp).Distinct().ToListAsync();

            return View(sanPhams);
        }

        public async Task<IActionResult> Them()
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();
            ViewBag.Categories = await _context.SanPhams.Select(s => s.LoaiSp).Distinct().ToListAsync();
            return View(new SanPham());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(SanPham model, IFormFile? hinhAnhFile)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            if (await _context.SanPhams.AnyAsync(s => s.MaSp == model.MaSp))
            {
                ModelState.AddModelError("MaSp", "Mã sản phẩm đã tồn tại");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.SanPhams.Select(s => s.LoaiSp).Distinct().ToListAsync();
                return View(model);
            }

            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                model.HinhAnh = await LuuHinhAnh(hinhAnhFile);

            model.TrangThai = 1;
            _context.SanPhams.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sua(string id)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            ViewBag.Categories = await _context.SanPhams.Select(s => s.LoaiSp).Distinct().ToListAsync();
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(string id, SanPham model, IFormFile? hinhAnhFile)
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();
            if (id != model.MaSp) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.SanPhams.Select(s => s.LoaiSp).Distinct().ToListAsync();
                return View(model);
            }

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            sp.TenSp = model.TenSp;
            sp.LoaiSp = model.LoaiSp;
            sp.GiaBan = model.GiaBan;
            sp.SoLuongTon = model.SoLuongTon;
            sp.TrangThai = model.TrangThai;

            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                sp.HinhAnh = await LuuHinhAnh(hinhAnhFile);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(string id)
        {
            if (!IsAdminLoggedIn()) return Json(new { success = false });

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return Json(new { success = false });

            // Toggle trạng thái ẩn / hiện
            sp.TrangThai = (sp.TrangThai == 1) ? 0 : 1;
            await _context.SaveChangesAsync();
            return Json(new { success = true, trangThai = sp.TrangThai });
        }

        private async Task<string> LuuHinhAnh(IFormFile file)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "images", "sanpham");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/sanpham/{fileName}";
        }
    }
}
