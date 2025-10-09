using System.ComponentModel.DataAnnotations;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace EmployeeRightsManagement.Models
{
    public sealed class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Position { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
        
        public string FullName => $"{FirstName} {LastName}";
    }
}
