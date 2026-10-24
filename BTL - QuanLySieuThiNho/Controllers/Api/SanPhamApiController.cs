using BTL___QuanLySieuThiNho.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SanPhamApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SanPhamApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSanPhams(string? timKiem, string? loaiSp)
        {
            var query = _context.SanPhams.Where(s => s.TrangThai == null || s.TrangThai == 1).AsQueryable();

            if (!string.IsNullOrWhiteSpace(timKiem))
                query = query.Where(s => s.TenSp.Contains(timKiem));

            if (!string.IsNullOrWhiteSpace(loaiSp))
                query = query.Where(s => s.LoaiSp == loaiSp);

            var result = await query.Select(s => new
            {
                s.MaSp,
                TenSanPham = s.TenSp,
                s.LoaiSp,
                Gia = s.GiaBan,
                SoLuong = s.SoLuongTon,
                s.HinhAnh
            }).ToListAsync();

            return Ok(result);
        }

        // GET: api/SanPhamApi/SP001
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetSanPham(string id)
        {
            var sanPham = await _context.SanPhams
                .Where(s => s.MaSp == id)
                .Select(s => new
                {
                    s.MaSp,
                    TenSanPham = s.TenSp,
                    s.LoaiSp,
                    Gia = s.GiaBan,
                    SoLuong = s.SoLuongTon,
                    s.HinhAnh,
                    s.TrangThai
                })
                .FirstOrDefaultAsync();

            if (sanPham == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm" });

            return Ok(sanPham);
        }
    }
}
