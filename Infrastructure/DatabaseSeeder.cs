using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Employees.Any())
            {
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

                var roles = new[]
                {
                    new Role { Id = 1, Name = "Administrator", Description = "Full system access", IsActive = true, CreatedDate = DateTime.Now.AddDays(-30) },
                    new Role { Id = 2, Name = "Manager", Description = "Management level access", IsActive = true, CreatedDate = DateTime.Now.AddDays(-25) },
                    new Role { Id = 3, Name = "Employee", Description = "Basic employee access", IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) },
                    new Role { Id = 4, Name = "HR Manager", Description = "Human Resources management", IsActive = true, CreatedDate = DateTime.Now.AddDays(-15) },
                    new Role { Id = 5, Name = "IT Support", Description = "IT support and maintenance", IsActive = true, CreatedDate = DateTime.Now.AddDays(-10) }
                };

                context.Roles.AddRange(roles);

                var employees = new[]
                {
                    new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@company.com", Department = "IT", Position = "System Administrator", IsActive = true, CreatedDate = DateTime.Now.AddDays(-30) },
                    new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@company.com", Department = "HR", Position = "HR Manager", IsActive = true, CreatedDate = DateTime.Now.AddDays(-25) },
                    new Employee { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@company.com", Department = "Finance", Position = "Financial Analyst", IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) },
                    new Employee { Id = 4, FirstName = "Alice", LastName = "Brown", Email = "alice.brown@company.com", Department = "IT", Position = "Developer", IsActive = true, CreatedDate = DateTime.Now.AddDays(-15) },
                    new Employee { Id = 5, FirstName = "Charlie", LastName = "Wilson", Email = "charlie.wilson@company.com", Department = "Operations", Position = "Operations Manager", IsActive = true, CreatedDate = DateTime.Now.AddDays(-10) }
                };

                context.Employees.AddRange(employees);

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

                context.SaveChanges();
            }
        }
    }
}

