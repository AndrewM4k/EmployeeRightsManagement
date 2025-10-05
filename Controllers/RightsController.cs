using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Controllers
{
    public class RightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRights()
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
                    r.Type,
                    r.IsActive,
                    r.CreatedDate
                })
                .ToListAsync();

            return Json(rights);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRights(string? name, string? category, string? type)
        {
            var query = _context.Rights.Where(r => r.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(r => r.Category.Contains(category));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(r => r.Type.Contains(type));
            }

            var rights = await query
                .OrderBy(r => r.Category)
                .ThenBy(r => r.Name)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.Category,
                    r.Type,
                    r.IsActive,
                    r.CreatedDate
                })
                .ToListAsync();

            return Json(rights);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRight([FromBody] Right right)
        {
            if (ModelState.IsValid)
            {
                right.CreatedDate = DateTime.Now;
                right.IsActive = true;
                _context.Rights.Add(right);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Right created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRight([FromBody] Right right)
        {
            if (ModelState.IsValid)
            {
                _context.Rights.Update(right);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Right updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRight(int id)
        {
            var right = await _context.Rights.FindAsync(id);
            if (right != null)
            {
                right.IsActive = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Right deleted successfully" });
            }
            return Json(new { success = false, message = "Right not found" });
        }
    }
}
