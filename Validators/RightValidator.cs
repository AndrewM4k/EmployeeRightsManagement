using EmployeeRightsManagement.Models;
using FluentValidation;

namespace EmployeeRightsManagement.Validators
{
    public class RightValidator : AbstractValidator<Right>
    {
        public RightValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
        }
    }
}




