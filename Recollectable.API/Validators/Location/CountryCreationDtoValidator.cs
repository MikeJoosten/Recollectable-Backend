using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Locations;

namespace Recollectable.API.Validators.Location
{
    public class CountryCreationDtoValidator : CountryManipulationDtoValidator<CountryCreationDto>
    {
        public CountryCreationDtoValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}