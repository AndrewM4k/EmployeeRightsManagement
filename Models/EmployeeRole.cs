namespace EmployeeRightsManagement.Models
{// ReSharper disable PropertyCanBeMadeInitOnly.Global

    public sealed class EmployeeRole
    {
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public Employee Employee { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
