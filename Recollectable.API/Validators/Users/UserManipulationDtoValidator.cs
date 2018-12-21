using FluentValidation;
using Recollectable.API.Models.Users;

namespace Recollectable.API.Validators.Users
{
    public class UserManipulationDtoValidator<T> : AbstractValidator<T>
        where T : UserManipulationDto
    {
        public UserManipulationDtoValidator()
        {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("UserName is a required field")
                .MaximumLength(50).WithMessage("UserName shouldn't contain more than 50 characters");

            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("FirstName is a required field")
                .MaximumLength(100).WithMessage("FirstName shouldn't contain more than 100 characters");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("LastName is a required field")
                .MaximumLength(100).WithMessage("LastName shouldn't contain more than 100 characters");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is a required field")
                .MaximumLength(250).WithMessage("Email shouldn't contain more than 250 characters")
                .EmailAddress();
        }
    }
}