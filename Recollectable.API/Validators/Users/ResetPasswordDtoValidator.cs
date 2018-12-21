using FluentValidation;
using Recollectable.API.Models.Users;

namespace Recollectable.API.Validators.Users
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("Password is a required field");

            RuleFor(p => p.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmationPassword is a required field")
                .Equal(p => p.Password).WithMessage("Password and ConfirmationPassword must be equal");
        }
    }
}