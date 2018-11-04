using FluentValidation;
using Recollectable.Core.Models.Users;

namespace Recollectable.API.Validators.Users
{
    public class UserCreationDtoValidator : UserManipulationDtoValidator<UserCreationDto>
    {
        public UserCreationDtoValidator()
        {
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is a required field");
        }
    }
}