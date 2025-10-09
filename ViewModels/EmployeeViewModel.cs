// ReSharper disable ClassNeverInstantiated.Global
namespace EmployeeRightsManagement.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public List<RightViewModel> AllRights { get; set; } = new List<RightViewModel>();
    }

    public class RoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<RightViewModel> Rights { get; set; } = new List<RightViewModel>();
    }

    public class RightViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsAssigned { get; set; }
    }
}
