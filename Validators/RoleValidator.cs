using EmployeeRightsManagement.Models;
using FluentValidation;

namespace EmployeeRightsManagement.Validators
{
    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        }
    }
}




