using Recollectable.API.Models.Locations;
using Recollectable.Core.Interfaces;

namespace Recollectable.API.Validators.Location
{
    public class CountryUpdateDtoValidator : CountryManipulationDtoValidator<CountryUpdateDto>
    {
        public CountryUpdateDtoValidator(ICountryService service)
            : base(service)
        {
        }
    }
}