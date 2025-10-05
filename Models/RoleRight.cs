namespace EmployeeRightsManagement.Models
{
    public class RoleRight
    {
        public int RoleId { get; set; }
        public int RightId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Right Right { get; set; } = null!;
    }
}
