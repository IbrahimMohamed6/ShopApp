using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopApp.BLL.Extensions;
using ShopApp.DAL.Data;
using ShopApp.DAL.Extensions;
using ShopApp.DAL.Models;

var builder = WebApplication.CreateBuilder(args);

// ════════════════════════════════════════════════════════════════════════════
//  1.  DATA ACCESS LAYER — registers DbContext + all Repositories
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddDAL(builder.Configuration);

// ════════════════════════════════════════════════════════════════════════════
//  2.  BUSINESS LOGIC LAYER — registers all Services
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddBLL();

// ════════════════════════════════════════════════════════════════════════════
//  3.  IDENTITY (lives in PL since it needs UI pages)
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// ════════════════════════════════════════════════════════════════════════════
//  4.  SESSION (for cart)
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout      = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly  = true;
    options.Cookie.IsEssential = true;
});

// ════════════════════════════════════════════════════════════════════════════
//  5.  MVC + Razor Pages (Identity UI)
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ════════════════════════════════════════════════════════════════════════════
var app = builder.Build();
// ════════════════════════════════════════════════════════════════════════════

// ── Migrate & Seed ────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        await DbSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error during DB migration / seeding.");
    }
}

// ── Middleware Pipeline ───────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();           // ← before UseAuthentication
app.UseAuthentication();
app.UseAuthorization();

// ── Route Mapping ─────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
