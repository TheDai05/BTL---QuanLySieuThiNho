using BTL___QuanLySieuThiNho.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key for ChiTietHoaDon
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasKey(ct => new { ct.MaHd, ct.MaSp });

            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(ct => ct.ThanhTien)
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(ct => ct.HoaDon)
                .WithMany(h => h.ChiTietHoaDons)
                .HasForeignKey(ct => ct.MaHd);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(ct => ct.SanPham)
                .WithMany(s => s.ChiTietHoaDons)
                .HasForeignKey(ct => ct.MaSp);

            modelBuilder.Entity<GioHang>()
                .HasOne(g => g.KhachHang)
                .WithMany(k => k.GioHangs)
                .HasForeignKey(g => g.MaKh);

            modelBuilder.Entity<GioHang>()
                .HasOne(g => g.SanPham)
                .WithMany(s => s.GioHangs)
                .HasForeignKey(g => g.MaSp);

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.KhachHang)
                .WithMany(k => k.HoaDons)
                .HasForeignKey(h => h.MaKh);

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.NhanVien)
                .WithMany(n => n.HoaDons)
                .HasForeignKey(h => h.MaNv);
        }
    }
}
