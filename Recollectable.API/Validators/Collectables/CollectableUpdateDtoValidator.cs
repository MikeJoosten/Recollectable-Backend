using FluentValidation;
using Recollectable.Core.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class CollectableUpdateDtoValidator : CollectableManipulationDtoValidator<CollectableUpdateDto>
    {
        public CollectableUpdateDtoValidator()
        {
            RuleFor(c => c.CollectionId)
                .NotEmpty().WithMessage("CollectionId is a required field");
        }
    }
}