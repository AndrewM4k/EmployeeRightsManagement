using System.ComponentModel.DataAnnotations;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace EmployeeRightsManagement.Models
{
    public sealed class Role
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
        public ICollection<RoleRight> RoleRights { get; set; } = new List<RoleRight>();
    }
}
