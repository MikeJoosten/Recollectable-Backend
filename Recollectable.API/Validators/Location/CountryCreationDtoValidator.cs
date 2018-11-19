using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Locations;

namespace Recollectable.API.Validators.Location
{
    public class CountryCreationDtoValidator : CountryManipulationDtoValidator<CountryCreationDto>
    {
        public CountryCreationDtoValidator(ICountryRepository repository)
            : base(repository)
        {
        }
    }
}