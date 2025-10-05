using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.ViewModels;

namespace EmployeeRightsManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult MyRights()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRights()
        {
            // For demo purposes, we'll get rights for the first employee
            // In a real application, this would be based on the current logged-in user
            var employeeId = 1; // This should come from the current user's context

            var employeeRights = await _context.Employees
                .Where(e => e.Id == employeeId)
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                        .ThenInclude(r => r.RoleRights)
                            .ThenInclude(rr => rr.Right)
                .Select(e => new
                {
                    Employee = new
                    {
                        e.Id,
                        e.FullName,
                        e.Email,
                        e.Department,
                        e.Position
                    },
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
                                    rr.Right.Type,
                                    rr.AssignedDate
                                })
                        }),
                    AllRights = e.EmployeeRoles
                        .Where(er => er.IsActive)
                        .SelectMany(er => er.Role.RoleRights)
                        .Where(rr => rr.IsActive)
                        .Select(rr => new
                        {
                            rr.Right.Id,
                            rr.Right.Name,
                            rr.Right.Description,
                            rr.Right.Category,
                            rr.Right.Type,
                            AssignedDate = rr.AssignedDate,
                            RoleName = rr.Role.Name
                        })
                        .Distinct()
                })
                .FirstOrDefaultAsync();

            return Json(employeeRights);
        }
    }
}
