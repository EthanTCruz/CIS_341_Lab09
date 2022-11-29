using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Lab8.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CommunityStoreContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var connectionString = builder.Configuration.GetConnectionString("AuthenticationContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthenticationContextConnection' not found.");

builder.Services.AddDbContext<AuthenticationContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AuthenticationContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();



// Modify authentication options. You can also change the user, signin and lockout settings.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Service provider for the scope
    var services = scope.ServiceProvider;
    try
    {
        // Get the DbContext from the service provider
        var context = services.GetRequiredService<CommunityStoreContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
