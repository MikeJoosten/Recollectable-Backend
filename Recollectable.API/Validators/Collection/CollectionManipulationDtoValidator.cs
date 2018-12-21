using FluentValidation;
using Recollectable.API.Models.Collections;

namespace Recollectable.API.Validators.Collection
{
    public class CollectionManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CollectionManipulationDto
    {
        public CollectionManipulationDtoValidator()
        {
            RuleFor(c => c.Type)
                .NotEmpty().WithMessage("Type is a required field")
                .MaximumLength(25).WithMessage("Type shouldn't contain more than 25 characters");

            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("UserId is a required field");
        }
    }
}