using Microsoft.AspNetCore.Mvc;
using EmployeeRightsManagement.Services;
using EmployeeRightsManagement.Services.Users;

namespace EmployeeRightsManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserContext _currentUser;

        // ReSharper disable once ConvertToPrimaryConstructor
        public UserController(IUserService userService, ICurrentUserContext currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
        }

        public IActionResult MyRights()
        {
            ViewBag.IsAdmin = _currentUser.IsAdmin;
            return View();
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> GetMyRights()
        {
            var result = await _userService.GetMyRightsAsync(_currentUser.IsAdmin);
            return Json(result);
        }
    }
}




