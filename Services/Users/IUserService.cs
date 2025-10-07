namespace EmployeeRightsManagement.Services.Users
{
    public interface IUserService
    {
        Task<object?> GetMyRightsAsync(bool isAdmin);
    }
}




