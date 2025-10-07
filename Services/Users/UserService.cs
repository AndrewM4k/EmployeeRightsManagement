using EmployeeRightsManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRightsManagement.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<object?> GetMyRightsAsync(bool isAdmin)
        {
            int targetEmployeeId = isAdmin ? 1 : 3;

            var employee = await _dbContext.Employees
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
                var fallbackQuery = isAdmin
                    ? _dbContext.EmployeeRoles.AsNoTracking().Where(er => er.IsActive && er.Role.Name == "Administrator")
                    : _dbContext.EmployeeRoles.AsNoTracking().Where(er => er.IsActive && er.Role.Name != "Administrator");

                targetEmployeeId = await fallbackQuery
                    .OrderBy(er => er.EmployeeId)
                    .Select(er => er.EmployeeId)
                    .FirstOrDefaultAsync();

                employee = await _dbContext.Employees.AsNoTracking()
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
                return null;
            }

            var roles = await _dbContext.EmployeeRoles
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

            var rightsByRoleFlat = await _dbContext.RoleRights
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

            var allRights = await _dbContext.RoleRights
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

            return new
            {
                employee,
                roles = rolesWithRights,
                allRights
            };
        }
    }
}




