using BTL___QuanLySieuThiNho.Data;
using BTL___QuanLySieuThiNho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers.Admin
{
    public class QuanLyDanhMucController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public QuanLyDanhMucController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn()) return RequireAdminLogin();

            var categories = await _context.SanPhams
                .Select(s => s.LoaiSp)
                .Distinct()
                .Select(cat => new
                {
                    TenDanhMuc = cat,
                    SoSanPham = _context.SanPhams.Count(s => s.LoaiSp == cat)
                })
                .ToListAsync();

            ViewBag.Categories = categories;
            return View();
        }
    }
}
