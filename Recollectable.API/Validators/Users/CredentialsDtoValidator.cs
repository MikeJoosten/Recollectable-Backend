using FluentValidation;
using Recollectable.Core.Models.Users;

namespace Recollectable.API.Validators.Users
{
    public class CredentialsDtoValidator : AbstractValidator<CredentialsDto>
    {
        public CredentialsDtoValidator()
        {
            RuleFor(c => c.UserName)
                .NotEmpty().WithMessage("UserName is a required field")
                .MaximumLength(50).WithMessage("UserName shouldn't contain more than 50 characters");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Password is a required field");
        }
    }
}