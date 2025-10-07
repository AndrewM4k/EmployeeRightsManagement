using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.Services.Employees;
using EmployeeRightsManagement.ViewModels;

namespace EmployeeRightsManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserContext _currentUser;

        public EmployeeController(IEmployeeService employeeService, ICurrentUserContext currentUser)
        {
            _employeeService = employeeService;
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
            var employees = await _employeeService.GetEmployeesAsync();

            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeDetailsAsync(id);

            return Json(employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _employeeService.GetAllRolesAsync();

            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data" });
            var result = await _employeeService.CreateEmployeeAsync(employee);
            return Json(new { success = result.success, message = result.message, employeeId = result.employeeId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid data" });
            var result = await _employeeService.UpdateEmployeeAsync(employee);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            var result = await _employeeService.DeleteEmployeeAsync(id);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRolesToEmployee([FromBody] AssignRolesRequest request)
        {
            if (!_currentUser.IsAdmin) return Forbid();
            try
            {
                var result = await _employeeService.AssignRolesAsync(request.EmployeeId, request.RoleIds);
                return Json(new { success = result.success, message = result.message });
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



