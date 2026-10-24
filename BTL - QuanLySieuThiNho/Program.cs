using BTL___QuanLySieuThiNho.Data;
using Microsoft.EntityFrameworkCore;

namespace BTL___QuanLySieuThiNho
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    // Đảm bảo Razor luôn tìm thấy view ở cả thư mục Views chuẩn lẫn Areas
                    options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
                });

            // EF Core + SQL Server (Database QuanLySieuThiNho có sẵn)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Name = ".QuanLySieuThi.Session";
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            // Route Admin DangNhap
            app.MapControllerRoute(
                name: "admin_login",
                pattern: "Admin/DangNhap",
                defaults: new { controller = "AdminAccount", action = "DangNhap" });

            // Route Admin (chỉ áp dụng cho các Controller quản trị)
            app.MapControllerRoute(
                name: "admin",
                pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
                constraints: new { controller = @"(Dashboard|AdminAccount|QuanLySanPham|QuanLyDanhMuc|QuanLyDonHang|QuanLyKhachHang)" });

            // Route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
