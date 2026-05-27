using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Data;
using PicGramWebApp.Models;
using PicGramWebApp.Services.Adapters;
using PicGramWebApp.Services.Facade;
using PicGramWebApp.Services.Logging;
using PicGramWebApp.Services.Observers;
using PicGramWebApp.Services.Packages;
using PicGramWebApp.Services.Search;
using PicGramWebApp.Services.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddAuthentication()
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
        options.Scope.Add("user:email");
    });

builder.Services.AddScoped<IPhotoActionObserver, ActionLogObserver>();
builder.Services.AddScoped<PhotoActionSubject>();
builder.Services.AddScoped<PhotoFacade>();
builder.Services.AddScoped<LocalStorageProvider>();
builder.Services.AddScoped<StorageProviderFactory>();
builder.Services.AddScoped<PackageLimitService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AppActionLogger>();
builder.Services.AddScoped<PackageChangeService>();
builder.Services.AddScoped<IExternalUserAdapter, GitHubExternalUserAdapter>();
builder.Services.AddScoped<IPhotoSearchService, PhotoSearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // 1. Seed packages
    if (!db.PackagePlans.Any())
    {
        db.PackagePlans.AddRange(
            new PackagePlan
            {
                Name = "FREE",
                Price = 0,
                MaxUploadsPerMonth = 5,
                MaxStorageBytes = 100 * 1024 * 1024,
                MaxDownloadsPerMonth = 10
            },
            new PackagePlan
            {
                Name = "PRO",
                Price = 9.99m,
                MaxUploadsPerMonth = 50,
                MaxStorageBytes = 1024L * 1024 * 1024,
                MaxDownloadsPerMonth = 100
            },
            new PackagePlan
            {
                Name = "GOLD",
                Price = 19.99m,
                MaxUploadsPerMonth = 500,
                MaxStorageBytes = 5L * 1024 * 1024 * 1024,
                MaxDownloadsPerMonth = 1000
            }
        );

        db.SaveChanges();
    }

    // 2. Seed roles
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 3. Promote existing user to Admin
    var adminEmail = "test1@gmail.com";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();
