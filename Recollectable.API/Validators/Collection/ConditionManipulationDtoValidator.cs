using FluentValidation;
using Recollectable.API.Models.Collections;

namespace Recollectable.API.Validators.Collection
{
    public class ConditionManipulationDtoValidator<T> : AbstractValidator<T>
        where T : ConditionManipulationDto
    {
        public ConditionManipulationDtoValidator()
        {
            RuleFor(c => c.Grade)
                .NotEmpty().WithMessage("Grade is a required field");

            RuleFor(c => c.LanguageCode)
                    .NotEmpty().WithMessage("LanguageCode is a required field");
        }
    }
}