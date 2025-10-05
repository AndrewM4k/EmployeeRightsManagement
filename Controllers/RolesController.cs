using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
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
            if (ModelState.IsValid)
            {
                role.CreatedDate = DateTime.Now;
                role.IsActive = true;
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Role created successfully", roleId = role.Id });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Role updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                role.IsActive = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Role deleted successfully" });
            }
            return Json(new { success = false, message = "Role not found" });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRightsToRole([FromBody] AssignRightsRequest request)
        {
            try
            {
                // Remove existing rights for this role
                var existingRights = await _context.RoleRights
                    .Where(rr => rr.RoleId == request.RoleId)
                    .ToListAsync();
                
                _context.RoleRights.RemoveRange(existingRights);

                // Add new rights
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
