using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.Services.Employees;
using EmployeeRightsManagement.Services.Rights;
using EmployeeRightsManagement.Services.Roles;
using EmployeeRightsManagement.Services.Users;
using EmployeeRightsManagement.Repositories;
using EmployeeRightsManagement.Middleware;
using Microsoft.AspNetCore.Mvc.Razor;
using FluentValidation;
using FluentValidation.AspNetCore;
using EmployeeRightsManagement.Validators;
using EmployeeRightsManagement.Options;
using EmployeeRightsManagement.Infrastructure;
using AutoMapper;
using EmployeeRightsManagement.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Demo current user context registration (role from config: Admin/User)
builder.Services.AddSingleton<ICurrentUserContext>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var role = cfg.GetValue<string>("DemoUser:Role") ?? "User";
    return new DemoCurrentUserContext(role);
});

// Add services to the container.
builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection(ConnectionStringsOptions.SectionName));
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var conn = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ConnectionStringsOptions>>().Value.DefaultConnection;
    options.UseSqlServer(conn);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddRazorPages();
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure Razor to resolve views exclusively from /frontend/Views
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/frontend/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/frontend/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/frontend/Areas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/frontend/Areas/{2}/Views/Shared/{0}.cshtml");
});

// Register repositories and services (DAL/BLL)
// Register FluentValidation validators from assembly containing our validators
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRightRepository, RightRepository>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRightService, RightService>();
builder.Services.AddScoped<IUserService, UserService>();

// Kendo UI is loaded via CDN

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Order: Error handling -> HTTPS -> Static files -> Routing -> Auth -> Caching headers -> ResponseCaching -> Endpoints
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Static files are now served from the configured WebRoot (frontend/wwwroot)

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ResponseCachingHeadersMiddleware>();
app.UseResponseCaching();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.InitializeDatabaseAsync();

app.Run();
