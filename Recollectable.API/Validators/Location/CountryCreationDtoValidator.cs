using Recollectable.API.Models.Locations;
using Recollectable.Core.Interfaces;

namespace Recollectable.API.Validators.Location
{
    public class CountryCreationDtoValidator : CountryManipulationDtoValidator<CountryCreationDto>
    {
        public CountryCreationDtoValidator(ICountryService service)
            : base(service)
        {
        }
    }
}