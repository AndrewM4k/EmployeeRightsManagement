using System.ComponentModel.DataAnnotations;

namespace EmployeeRightsManagement.Models
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
        public virtual ICollection<RoleRight> RoleRights { get; set; } = new List<RoleRight>();
    }
}
