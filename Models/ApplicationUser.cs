using Microsoft.AspNetCore.Identity;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace EmployeeRightsManagement.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
