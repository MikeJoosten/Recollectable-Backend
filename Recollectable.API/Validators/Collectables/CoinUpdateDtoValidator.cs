using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class CoinUpdateDtoValidator : CoinManipulationDtoValidator<CoinUpdateDto>
    {
        public CoinUpdateDtoValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}