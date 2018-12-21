using FluentValidation;
using Recollectable.API.Models.Collections;

namespace Recollectable.API.Validators.Collection
{
    public class CollectionCollectableManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CollectionCollectableManipulationDto
    {
        public CollectionCollectableManipulationDtoValidator()
        {
            RuleFor(c => c.CollectableId)
                .NotEmpty().WithMessage("CollectableId is a required field");

            RuleFor(c => c.Condition)
                .NotEmpty().WithMessage("Condition is a required field")
                .MaximumLength(50).WithMessage("Condition shouldn't contain more than 50 characters");
        }
    }
}
