namespace EmployeeRightsManagement.Services.Roles
{
    public interface IRoleService
    {
        Task<List<object>> GetRolesAsync();
        Task<object?> GetRoleAsync(int id);
        Task<(bool success, string message, int roleId)> CreateRoleAsync(object dto);
        Task<(bool success, string message)> UpdateRoleAsync(object dto);
        Task<(bool success, string message)> DeleteRoleAsync(int id);
        Task<(bool success, string message)> AssignRightsAsync(int roleId, IEnumerable<int> rightIds);
        Task<List<object>> GetAllRightsAsync();
    }
}




