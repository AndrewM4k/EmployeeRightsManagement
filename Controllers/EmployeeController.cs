using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.ViewModels;

namespace EmployeeRightsManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserContext _currentUser;

        public EmployeeController(ApplicationDbContext context, ICurrentUserContext currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public IActionResult Index()
        {
            if (!_currentUser.IsAdmin)
                return RedirectToAction("Index", "Home");
            ViewBag.IsAdmin = _currentUser.IsAdmin;
            return View();
        }

        public IActionResult Details(int id)
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
                .Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.FullName,
                    e.Email,
                    e.Department,
                    e.Position,
                    e.IsActive,
                    e.CreatedDate,
                    RolesCount = e.EmployeeRoles.Count(er => er.IsActive)
                })
                .ToListAsync();

            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                        .ThenInclude(r => r.RoleRights)
                            .ThenInclude(rr => rr.Right)
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.FullName,
                    e.Email,
                    e.Department,
                    e.Position,
                    e.IsActive,
                    e.CreatedDate,
                    Roles = e.EmployeeRoles
                        .Where(er => er.IsActive)
                        .Select(er => new
                        {
                            er.Role.Id,
                            er.Role.Name,
                            er.Role.Description,
                            er.AssignedDate,
                            Rights = er.Role.RoleRights
                                .Where(rr => rr.IsActive)
                                .Select(rr => new
                                {
                                    rr.Right.Id,
                                    rr.Right.Name,
                                    rr.Right.Description,
                                    rr.Right.Category,
                                    rr.Right.Type
                                })
                        })
                })
                .FirstOrDefaultAsync();

            return Json(employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles
                .Where(r => r.IsActive)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description
                })
                .ToListAsync();

            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (ModelState.IsValid)
            {
                employee.CreatedDate = DateTime.Now;
                employee.IsActive = true;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Employee created successfully", employeeId = employee.Id });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (ModelState.IsValid)
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Employee updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.IsActive = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Employee deleted successfully" });
            }
            return Json(new { success = false, message = "Employee not found" });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRolesToEmployee([FromBody] AssignRolesRequest request)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            try
            {
                // Remove existing roles for this employee
                var existingRoles = await _context.EmployeeRoles
                    .Where(er => er.EmployeeId == request.EmployeeId)
                    .ToListAsync();
                
                _context.EmployeeRoles.RemoveRange(existingRoles);

                // Add new roles
                var employeeRoles = request.RoleIds.Select(roleId => new EmployeeRole
                {
                    EmployeeId = request.EmployeeId,
                    RoleId = roleId,
                    AssignedDate = DateTime.Now,
                    IsActive = true
                });

                _context.EmployeeRoles.AddRange(employeeRoles);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Roles assigned successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error assigning roles: " + ex.Message });
            }
        }
    }

    public class AssignRolesRequest
    {
        public int EmployeeId { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}



