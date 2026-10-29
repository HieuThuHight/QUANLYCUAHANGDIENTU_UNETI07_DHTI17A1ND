// ============================================================================
// DỰ ÁN: QUẢN LÝ CỬA HÀNG ĐIỆN TỬ - UNETI (NHÓM 07 - DHTI17A1ND)
// PHÂN HỆ: PHẦN CHUNG (FOUNDATION & SHARED INFRASTRUCTURE)
// NHÓM SINH VIÊN THỰC HIỆN:
//   1. Đỗ Văn Hiếu    - 23203100018 - DHTI17A1ND - dvhieu.dhti17a1nd@sv.uneti.edu.vn (Trưởng nhóm)
//   2. Lưu Đức Huy    - 23274800002 - DHTI17A2ND - ldhuy.dhti17a2nd@sv.uneti.edu.vn
//   3. Vương Quang Đạt - 23203100036 - DHTI17A1ND - vqdat.dhti17a1nd@sv.uneti.edu.vn
//   4. Phạm Thái Hoàng - 23203100039 - DHTI17A1ND - pthoang.dhti17a1nd@sv.uneti.edu.vn
// NỘI DUNG: Cấu hình dịch vụ ứng dụng ASP.NET Core 10 MVC, EF Core SQL Server, Session, Authentication và Middleware
// ============================================================================

using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu.Data;
using QuanLyCuaHangDienTu.Services;

// Xác định ContentRootPath: Ưu tiên thư mục gốc project khi chạy trong môi trường phát triển (VS / dotnet run)
var projRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
var contentRoot = Directory.Exists(Path.Combine(projRoot, "wwwroot"))
    ? projRoot
    : AppContext.BaseDirectory;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot
});

// Đảm bảo WebRootPath luôn trỏ tới thư mục wwwroot chứa đầy đủ hình ảnh sản phẩm
var wwwrootCandidate = Path.Combine(contentRoot, "wwwroot");
if (Directory.Exists(wwwrootCandidate))
{
    builder.Environment.WebRootPath = wwwrootCandidate;
}
else if (!string.IsNullOrEmpty(builder.Environment.WebRootPath) && Directory.Exists(builder.Environment.WebRootPath))
{
    // Đã có WebRootPath hợp lệ
}
else
{
    var fallback = Path.Combine(AppContext.BaseDirectory, "wwwroot");
    if (Directory.Exists(fallback)) builder.Environment.WebRootPath = fallback;
}

// DbContext
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<PdfExportService>();
builder.Services.AddScoped<VoucherService>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi seed data");
    }
}

app.Run();