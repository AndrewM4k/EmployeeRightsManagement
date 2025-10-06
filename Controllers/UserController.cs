using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.ViewModels;

namespace EmployeeRightsManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserContext _currentUser;

        public UserController(ApplicationDbContext context, ICurrentUserContext currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public IActionResult MyRights()
        {
            ViewBag.IsAdmin = _currentUser.IsAdmin;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRights()
        {
            // Admin -> Id = 1; User -> Id = 3
            int targetEmployeeId = _currentUser.IsAdmin ? 1 : 3;

            // 1) Быстрый запрос на базовую информацию о сотруднике
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.IsActive && e.Id == targetEmployeeId)
                .Select(e => new
                {
                    id = e.Id,
                    fullName = e.FullName,
                    email = e.Email,
                    department = e.Department,
                    position = e.Position,
                    isActive = e.IsActive,
                    createdDate = e.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                // фолбэк: любой активный в зависимости от роли
                var fallbackQuery = _currentUser.IsAdmin
                    ? _context.EmployeeRoles.AsNoTracking().Where(er => er.IsActive && er.Role.Name == "Administrator")
                    : _context.EmployeeRoles.AsNoTracking().Where(er => er.IsActive && er.Role.Name != "Administrator");

                targetEmployeeId = await fallbackQuery
                    .OrderBy(er => er.EmployeeId)
                    .Select(er => er.EmployeeId)
                    .FirstOrDefaultAsync();

                employee = await _context.Employees.AsNoTracking()
                    .Where(e => e.IsActive && e.Id == targetEmployeeId)
                    .Select(e => new
                    {
                        id = e.Id,
                        fullName = e.FullName,
                        email = e.Email,
                        department = e.Department,
                        position = e.Position,
                        isActive = e.IsActive,
                        createdDate = e.CreatedDate
                    })
                    .FirstOrDefaultAsync();
            }

            if (employee == null)
            {
                return Json(null);
            }

            // 2) Роли сотрудника (легкий join, без Include)
            var roles = await _context.EmployeeRoles
                .AsNoTracking()
                .Where(er => er.EmployeeId == targetEmployeeId && er.IsActive && er.Role.IsActive)
                .Select(er => new
                {
                    id = er.RoleId,
                    name = er.Role.Name,
                    description = er.Role.Description,
                    assignedDate = er.AssignedDate
                })
                .ToListAsync();

            var roleIds = roles.Select(r => r.id).ToArray();

            // 3a) Права сгруппированные по ролям (для блока My Roles)
            var rightsByRoleFlat = await _context.RoleRights
                .AsNoTracking()
                .Where(rr => roleIds.Contains(rr.RoleId) && rr.IsActive && rr.Right.IsActive)
                .Select(rr => new
                {
                    roleId = rr.RoleId,
                    id = rr.Right.Id,
                    name = rr.Right.Name,
                    description = rr.Right.Description,
                    category = rr.Right.Category,
                    type = rr.Right.Type,
                    assignedDate = rr.AssignedDate
                })
                .ToListAsync();

            var rightsLookup = rightsByRoleFlat
                .GroupBy(x => x.roleId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => (object)new
                    {
                        id = r.id,
                        name = r.name,
                        description = r.description,
                        category = r.category,
                        type = r.type,
                        assignedDate = r.assignedDate
                    }).ToList()
                );

            var rolesWithRights = roles
                .Select(r => new
                {
                    r.id,
                    r.name,
                    r.description,
                    r.assignedDate,
                    rights = rightsLookup.ContainsKey(r.id) ? rightsLookup[r.id] : new List<object>()
                })
                .ToList();

            // 3b) Агрегированные права по ролям (без тяжелых графов)
            var allRights = await _context.RoleRights
                .AsNoTracking()
                .Where(rr => roleIds.Contains(rr.RoleId) && rr.IsActive && rr.Right.IsActive)
                .Select(rr => new
                {
                    id = rr.Right.Id,
                    name = rr.Right.Name,
                    description = rr.Right.Description,
                    category = rr.Right.Category,
                    type = rr.Right.Type,
                    assignedDate = rr.AssignedDate,
                    roleName = rr.Role.Name
                })
                .Distinct()
                .ToListAsync();

            var result = new
            {
                employee,
                roles = rolesWithRights,
                allRights
            };

            return Json(result);
        }
    }
}




