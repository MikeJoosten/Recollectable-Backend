using FluentValidation;
using Recollectable.API.Models.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Extensions;

namespace Recollectable.API.Validators.Location
{
    public class CountryManipulationDtoValidator<T> : AbstractValidator<T>
        where T : CountryManipulationDto
    {
        private readonly ICountryService _service;

        public CountryManipulationDtoValidator(ICountryService service)
        {
            _service = service;

            var countries = _service.FindCountries(new CountriesResourceParameters()).Result;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is a required field")
                .MaximumLength(100).WithMessage("Name shouldn't contain more than 100 characters")
                .IsUnique(countries).WithMessage("Name must be unique");
        }
    }
}