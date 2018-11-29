using FluentValidation;
using Recollectable.API.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class CollectableManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CollectionCollectableManipulationDto
    {
        public CollectableManipulationDtoValidator()
        {
            RuleFor(c => c.CollectableId)
                .NotEmpty().WithMessage("CollectableId is a required field");

            RuleFor(c => c.Condition)
                .NotEmpty().WithMessage("Condition is a required field")
                .MaximumLength(50).WithMessage("Condition shouldn't contain more than 50 characters");
        }
    }
}