namespace EmployeeRightsManagement.Services
{
    public interface ICurrentUserContext
    {
        bool IsAdmin { get; }
        string RoleName { get; }
    }

    public sealed class DemoCurrentUserContext : ICurrentUserContext
    {
        public bool IsAdmin { get; }
        public string RoleName { get; }

        public DemoCurrentUserContext(string roleName)
        {
            RoleName = roleName;
            IsAdmin = string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}











