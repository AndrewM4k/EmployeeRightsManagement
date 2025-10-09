namespace EmployeeRightsManagement.Models
{// ReSharper disable PropertyCanBeMadeInitOnly.Global

    public sealed class RoleRight
    {
        public int RoleId { get; set; }
        public int RightId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
        public Role Role { get; set; } = null!;
        public Right Right { get; set; } = null!;
    }
}
