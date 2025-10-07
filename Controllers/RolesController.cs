using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.Services.Roles;

namespace EmployeeRightsManagement.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ICurrentUserContext _currentUser;

        public RolesController(IRoleService roleService, ICurrentUserContext currentUser)
        {
            _roleService = roleService;
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
            var roles = await _roleService.GetRolesAsync();

            return Json(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var role = await _roleService.GetRoleAsync(id);

            return Json(role);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRights()
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var rights = await _roleService.GetAllRightsAsync();

            return Json(rights);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data" });
            var result = await _roleService.CreateRoleAsync(role);
            return Json(new { success = result.success, message = result.message, roleId = result.roleId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] Role role)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data" });
            var result = await _roleService.UpdateRoleAsync(role);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var result = await _roleService.DeleteRoleAsync(id);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRightsToRole([FromBody] AssignRightsRequest request)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            try
            {
                var result = await _roleService.AssignRightsAsync(request.RoleId, request.RightIds);
                return Json(new { success = result.success, message = result.message });
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



