using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.ViewModels;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.Services.Employees;

namespace EmployeeRightsManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserContext _currentUser;

        public HomeController(IEmployeeService employeeService, ICurrentUserContext currentUser)
        {
            _employeeService = employeeService;
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
            var employees = await _employeeService.GetEmployeesAsync();

            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string? name, string? role, string? right, [FromQuery] int[]? roleIds)
        {
            try
            {
                // Be flexible: support roleIds passed as repeated query params and/or comma/space-separated in a single value
                int[] effectiveRoleIds = roleIds ?? Array.Empty<int>();
                if (effectiveRoleIds.Length == 0)
                {
                    var raw = Request.Query["roleIds"]; // may contain multiple entries
                    var parsed = raw
                        .SelectMany(v => v.Split(new[] { ',', ' ', ';', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id!.Value)
                        .Distinct()
                        .ToArray();
                    if (parsed.Length > 0)
                    {
                        effectiveRoleIds = parsed;
                    }
                }

                var employees = await _employeeService.SearchEmployeesAsync(name, role, right, effectiveRoleIds);

                return Json(employees);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
