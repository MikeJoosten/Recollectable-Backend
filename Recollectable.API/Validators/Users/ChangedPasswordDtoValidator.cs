using FluentValidation;
using Recollectable.API.Models.Users;

namespace Recollectable.API.Validators.Users
{
    public class ChangedPasswordDtoValidator : AbstractValidator<ChangedPasswordDto>
    {
        public ChangedPasswordDtoValidator()
        {
            RuleFor(p => p.OldPassword)
                .NotEmpty().WithMessage("OldPassword is a required field");

            RuleFor(p => p.NewPassword)
                .NotEmpty().WithMessage("NewPassword is a required field");

            RuleFor(p => p.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmationPassword is a required field")
                .Equal(p => p.NewPassword).WithMessage("NewPassword and ConfirmationPassword must be equal");
        }
    }
}