using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class BanknoteUpdateDtoValidator : BanknoteManipulationDtoValidator<BanknoteUpdateDto>
    {
        public BanknoteUpdateDtoValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}