using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class BanknoteCreationDtoValidator : BanknoteManipulationDtoValidator<BanknoteCreationDto>
    {
        public BanknoteCreationDtoValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}