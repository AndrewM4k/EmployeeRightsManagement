using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Demo current user context registration (role from config: Admin/User)
builder.Services.AddSingleton<ICurrentUserContext>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var role = cfg.GetValue<string>("DemoUser:Role") ?? "User";
    return new DemoCurrentUserContext(role);
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Kendo UI is loaded via CDN

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    // Seed data if not already seeded
    if (!context.Employees.Any())
    {
        Console.WriteLine("No employees found. Seeding data...");
        SeedInitialData(context);
        Console.WriteLine("Data seeding completed.");
    }
    else
    {
        Console.WriteLine($"Found {context.Employees.Count()} employees in database.");
    }
}

app.Run();

// Seeding method
static void SeedInitialData(ApplicationDbContext context)
{
    // Seed Rights
    var rights = new List<Right>();
    var categories = new[] { "User Management", "System Administration", "Reports", "Workstation Management", "Security", "Data Access" };
    var types = new[] { "Read", "Write", "Delete", "Execute", "Admin" };

    for (int i = 1; i <= 200; i++)
    {
        rights.Add(new Right
        {
            Id = i,
            Name = $"Right_{i:D3}",
            Description = $"Description for Right {i}",
            Category = categories[i % categories.Length],
            Type = types[i % types.Length],
            IsActive = true,
            CreatedDate = DateTime.Now.AddDays(-i)
        });
    }

    context.Rights.AddRange(rights);

    // Seed Roles
    var roles = new[]
    {
        new Role { Id = 1, Name = "Administrator", Description = "Full system access", IsActive = true, CreatedDate = DateTime.Now.AddDays(-30) },
        new Role { Id = 2, Name = "Manager", Description = "Management level access", IsActive = true, CreatedDate = DateTime.Now.AddDays(-25) },
        new Role { Id = 3, Name = "Employee", Description = "Basic employee access", IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) },
        new Role { Id = 4, Name = "HR Manager", Description = "Human Resources management", IsActive = true, CreatedDate = DateTime.Now.AddDays(-15) },
        new Role { Id = 5, Name = "IT Support", Description = "IT support and maintenance", IsActive = true, CreatedDate = DateTime.Now.AddDays(-10) }
    };

    context.Roles.AddRange(roles);

    // Seed Employees
    var employees = new[]
    {
        new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@company.com", Department = "IT", Position = "System Administrator", IsActive = true, CreatedDate = DateTime.Now.AddDays(-30) },
        new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@company.com", Department = "HR", Position = "HR Manager", IsActive = true, CreatedDate = DateTime.Now.AddDays(-25) },
        new Employee { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@company.com", Department = "Finance", Position = "Financial Analyst", IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) },
        new Employee { Id = 4, FirstName = "Alice", LastName = "Brown", Email = "alice.brown@company.com", Department = "IT", Position = "Developer", IsActive = true, CreatedDate = DateTime.Now.AddDays(-15) },
        new Employee { Id = 5, FirstName = "Charlie", LastName = "Wilson", Email = "charlie.wilson@company.com", Department = "Operations", Position = "Operations Manager", IsActive = true, CreatedDate = DateTime.Now.AddDays(-10) }
    };

    context.Employees.AddRange(employees);

    // Seed EmployeeRoles
    var employeeRoles = new[]
    {
        new EmployeeRole { EmployeeId = 1, RoleId = 1, AssignedDate = DateTime.Now.AddDays(-30), IsActive = true },
        new EmployeeRole { EmployeeId = 2, RoleId = 4, AssignedDate = DateTime.Now.AddDays(-25), IsActive = true },
        new EmployeeRole { EmployeeId = 3, RoleId = 3, AssignedDate = DateTime.Now.AddDays(-20), IsActive = true },
        new EmployeeRole { EmployeeId = 4, RoleId = 5, AssignedDate = DateTime.Now.AddDays(-15), IsActive = true },
        new EmployeeRole { EmployeeId = 5, RoleId = 2, AssignedDate = DateTime.Now.AddDays(-10), IsActive = true }
    };

    context.EmployeeRoles.AddRange(employeeRoles);

    // Seed RoleRights (assigning some rights to roles)
    var roleRights = new List<RoleRight>();
    var random = new Random(42); 

    for (int roleId = 1; roleId <= 5; roleId++)
    {
        var rightsCount = roleId == 1 ? 200 : roleId == 2 ? 150 : roleId == 3 ? 50 : roleId == 4 ? 100 : 75;
        var selectedRights = Enumerable.Range(1, 200).OrderBy(x => random.Next()).Take(rightsCount);

        foreach (var rightId in selectedRights)
        {
            roleRights.Add(new RoleRight
            {
                RoleId = roleId,
                RightId = rightId,
                AssignedDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                IsActive = true
            });
        }
    }

    context.RoleRights.AddRange(roleRights);
    
    Console.WriteLine($"Seeding: {rights.Count} rights, {roles.Length} roles, {employees.Length} employees, {employeeRoles.Length} employee roles, {roleRights.Count} role rights");
    
    context.SaveChanges();
    Console.WriteLine("Database save completed successfully.");
}
