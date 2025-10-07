namespace EmployeeRightsManagement.Services.Rights
{
    public interface IRightService
    {
        Task<List<object>> GetRightsAsync(string? name, string? category, string? type);
        Task<(bool success, string message)> CreateRightAsync(object dto);
        Task<(bool success, string message)> UpdateRightAsync(object dto);
        Task<(bool success, string message)> DeleteRightAsync(int id);
    }
}




