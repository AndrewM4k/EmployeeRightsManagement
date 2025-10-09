using EmployeeRightsManagement.Models;
using FluentValidation;

namespace EmployeeRightsManagement.Validators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
            RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        }
    }
}




