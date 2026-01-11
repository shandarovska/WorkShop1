using Microsoft.EntityFrameworkCore;
using WorkShop1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WorkShop1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

await SeedRolesAndUsersAsync(app);

app.Run();

static async Task SeedRolesAndUsersAsync(WebApplication app) //LOGIN
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = new[] { "Admin", "Teacher", "Student" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    // Create a default admin user
    string adminEmail = "admin@rsweb.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Create a default teacher user
    string teacherEmail = "teacher@rsweb.com";
    var teacherUser = await userManager.FindByEmailAsync(teacherEmail);

    if (teacherUser == null)
    {
        teacherUser = new ApplicationUser
        {
            UserName = teacherEmail,
            Email = teacherEmail,
            EmailConfirmed = true,
            TeacherId = 1
        };
        await userManager.CreateAsync(teacherUser, "Teacher123!");
        await userManager.AddToRoleAsync(teacherUser, "Teacher");
    }
    // =========================
    // STUDENT 3 (SAFE + ENROLLED)
    // =========================
    string student3Email = "student3@rsweb.com";
    var student3User = await userManager.FindByEmailAsync(student3Email);

    if (student3User == null)
    {
        student3User = new ApplicationUser
        {
            UserName = student3Email,
            Email = student3Email,
            EmailConfirmed = true,
            StudentId = 3 
        };

        var result = await userManager.CreateAsync(student3User, "Student123!");
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    
    if (!await userManager.IsInRoleAsync(student3User, "Student"))
    {
        await userManager.AddToRoleAsync(student3User, "Student");
    }
}