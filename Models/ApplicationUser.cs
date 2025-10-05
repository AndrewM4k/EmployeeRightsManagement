using Microsoft.AspNetCore.Identity;

namespace EmployeeRightsManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
