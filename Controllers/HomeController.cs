using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.ViewModels;

namespace EmployeeRightsManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = e.FullName,
                    Email = e.Email,
                    Department = e.Department,
                    Position = e.Position,
                    IsActive = e.IsActive,
                    CreatedDate = e.CreatedDate,
                    Roles = e.EmployeeRoles
                        .Where(er => er.IsActive)
                        .Select(er => new RoleViewModel
                        {
                            Id = er.Role.Id,
                            Name = er.Role.Name,
                            Description = er.Role.Description,
                            IsActive = er.Role.IsActive
                        }).ToList()
                })
                .ToListAsync();

            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string? name, string? role, string? right)
        {
            var query = _context.Employees
                .Where(e => e.IsActive)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                        .ThenInclude(r => r.RoleRights)
                            .ThenInclude(rr => rr.Right)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FullName.Contains(name));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(e => e.EmployeeRoles
                    .Any(er => er.IsActive && er.Role.Name.Contains(role)));
            }

            if (!string.IsNullOrEmpty(right))
            {
                query = query.Where(e => e.EmployeeRoles
                    .Any(er => er.IsActive && er.Role.RoleRights
                        .Any(rr => rr.IsActive && rr.Right.Name.Contains(right))));
            }

            var employees = await query
                .Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FullName = e.FullName,
                    Email = e.Email,
                    Department = e.Department,
                    Position = e.Position,
                    IsActive = e.IsActive,
                    CreatedDate = e.CreatedDate,
                    Roles = e.EmployeeRoles
                        .Where(er => er.IsActive)
                        .Select(er => new RoleViewModel
                        {
                            Id = er.Role.Id,
                            Name = er.Role.Name,
                            Description = er.Role.Description,
                            IsActive = er.Role.IsActive
                        }).ToList()
                })
                .ToListAsync();

            return Json(employees);
        }
    }
}
