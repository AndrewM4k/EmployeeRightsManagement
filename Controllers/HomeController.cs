using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.ViewModels;
using EmployeeRightsManagement.Services;

namespace EmployeeRightsManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserContext _currentUser;

        public HomeController(ApplicationDbContext context, ICurrentUserContext currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public IActionResult Index()
        {
            if (!_currentUser.IsAdmin)
            {
                return RedirectToAction("MyRights", "User");
            }
            ViewBag.IsAdmin = _currentUser.IsAdmin;
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
        public async Task<IActionResult> SearchEmployees(string? name, string? role, string? right, [FromQuery] int[]? roleIds)
        {
            try
            {
                var query = _context.Employees
                    .Where(e => e.IsActive)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));
                }

                if (!string.IsNullOrEmpty(role))
                {
                    query = query.Where(e => e.EmployeeRoles
                        .Any(er => er.IsActive && er.Role.Name.Contains(role)));
                }

                if (roleIds != null && roleIds.Length > 0)
                {
                    query = query.Where(e => e.EmployeeRoles
                        .Any(er => er.IsActive && roleIds.Contains(er.RoleId)));
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
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
