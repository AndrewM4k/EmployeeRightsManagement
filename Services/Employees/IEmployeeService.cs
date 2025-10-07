using EmployeeRightsManagement.DTOs;

namespace EmployeeRightsManagement.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<EmployeeListDto>> GetEmployeesAsync();
        Task<EmployeeDetailsDto?> GetEmployeeDetailsAsync(int id);
        Task<List<RoleListDto>> GetAllRolesAsync();
        Task<(bool success, string message, int employeeId)> CreateEmployeeAsync(object dto);
        Task<(bool success, string message)> UpdateEmployeeAsync(object dto);
        Task<(bool success, string message)> DeleteEmployeeAsync(int id);
        Task<(bool success, string message)> AssignRolesAsync(int employeeId, IEnumerable<int> roleIds);
        Task<List<EmployeeWithRolesDto>> SearchEmployeesAsync(string? name, string? role, string? right, int[]? roleIds);
    }
}


