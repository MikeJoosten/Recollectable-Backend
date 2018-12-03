using FluentValidation;
using Recollectable.API.Models.Collections;

namespace Recollectable.API.Validators.Collection
{
    public class CollectionCollectableUpdateDtoValidator : 
        CollectionCollectableManipulationDtoValidator<CollectionCollectableUpdateDto>
    {
        public CollectionCollectableUpdateDtoValidator()
        {
            RuleFor(c => c.CollectionId)
                .NotEmpty().WithMessage("CollectionId is a required field");
        }
    }
}