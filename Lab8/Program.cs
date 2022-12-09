using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Lab8.Data;
using System.Reflection;
using Lab8.Models.DTO;

//EntityFrameworkCore\Update-Database -Context "AuthenticationContext"

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CommunityStoreApi", Version = "v1" });
    // Required to include XML comments generated during the build process.
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AuthenticationContext>();

    context.Database.Migrate();

    try
    {

        InitializeUsersRoles.Initialize(services).Wait();
    }
    catch (Exception ex)
    {
        // Something went wrong
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex.Message, "An error occurred seeding the users and roles.");
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
app.MapGet("/todoitems", async (CommunityStoreContext context) =>
{
    return await context.Listings.Select(t => new ListingDTO(t)).ToListAsync();
})
    .Produces<List<ListingDTO>>(StatusCodes.Status200OK);

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
