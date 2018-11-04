using FluentValidation;
using Recollectable.Core.Models.Locations;

namespace Recollectable.API.Validators.Location
{
    public class CountryManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CountryManipulationDto
    {
        public CountryManipulationDtoValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is a required field")
                .MaximumLength(100).WithMessage("Name shouldn't contain more than 100 characters");
        }
    }
}