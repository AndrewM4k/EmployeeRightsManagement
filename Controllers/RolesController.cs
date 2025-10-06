using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;

namespace EmployeeRightsManagement.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserContext _currentUser;

        public RolesController(ApplicationDbContext context, ICurrentUserContext currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public IActionResult Index()
        {
            if (!_currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.IsAdmin = _currentUser.IsAdmin;
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (!_currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var roles = await _context.Roles
                .Where(r => r.IsActive)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.IsActive,
                    r.CreatedDate,
                    RightsCount = r.RoleRights.Count(rr => rr.IsActive)
                })
                .ToListAsync();

            return Json(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var role = await _context.Roles
                .Include(r => r.RoleRights)
                    .ThenInclude(rr => rr.Right)
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.IsActive,
                    r.CreatedDate,
                    Rights = r.RoleRights
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
                })
                .FirstOrDefaultAsync();

            return Json(role);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRights()
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var rights = await _context.Rights
                .Where(r => r.IsActive)
                .OrderBy(r => r.Category)
                .ThenBy(r => r.Name)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.Category,
                    r.Type
                })
                .ToListAsync();

            return Json(rights);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            role.CreatedDate = DateTime.Now;
            role.IsActive = true;
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Role created successfully", roleId = role.Id });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] Role role)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Role updated successfully" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return Json(new { success = false, message = "Role not found" });
            }

            role.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Role deleted successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRightsToRole([FromBody] AssignRightsRequest request)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            try
            {
                var existingRights = await _context.RoleRights
                    .Where(rr => rr.RoleId == request.RoleId)
                    .ToListAsync();
                _context.RoleRights.RemoveRange(existingRights);

                var roleRights = request.RightIds.Select(rightId => new RoleRight
                {
                    RoleId = request.RoleId,
                    RightId = rightId,
                    AssignedDate = DateTime.Now,
                    IsActive = true
                });

                _context.RoleRights.AddRange(roleRights);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rights assigned successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error assigning rights: " + ex.Message });
            }
        }
    }

    public class AssignRightsRequest
    {
        public int RoleId { get; set; }
        public List<int> RightIds { get; set; } = new List<int>();
    }
}



