// Họ và tên: Đỗ Văn Hiếu
// Mã sinh viên: 23203100018
// Nội dung thực hiện: Cấu hình Program.cs

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