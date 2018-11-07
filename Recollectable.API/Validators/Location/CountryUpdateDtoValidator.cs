using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Locations;

namespace Recollectable.API.Validators.Location
{
    public class CountryUpdateDtoValidator : CountryManipulationDtoValidator<CountryUpdateDto>
    {
        public CountryUpdateDtoValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}