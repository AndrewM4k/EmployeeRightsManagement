using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.Services.Rights;

namespace EmployeeRightsManagement.Controllers
{
    public class RightsController : Controller
    {
        private readonly IRightService _rightService;
        private readonly ICurrentUserContext _currentUser;

        public RightsController(IRightService rightService, ICurrentUserContext currentUser)
        {
            _rightService = rightService;
            _currentUser = currentUser;
        }

        public IActionResult Index()
        {
            if (!_currentUser.IsAdmin)
                return RedirectToAction("Index", "Home");
            ViewBag.IsAdmin = _currentUser.IsAdmin;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRights()
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var rights = await _rightService.GetRightsAsync(null, null, null);

            return Json(rights);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRights(string? name, string? category, string? type)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var rights = await _rightService.GetRightsAsync(name, category, type);

            return Json(rights);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRight([FromBody] Right right)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data" });
            var result = await _rightService.CreateRightAsync(right);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRight([FromBody] Right right)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data" });
            var result = await _rightService.UpdateRightAsync(right);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRight(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var result = await _rightService.DeleteRightAsync(id);
            return Json(new { success = result.success, message = result.message });
        }
    }
}

