using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;

namespace Recollectable.API.Validators.Collectables
{
    public class CoinCreationDtoValidator : CoinManipulationDtoValidator<CoinCreationDto>
    {
        public CoinCreationDtoValidator(ICoinRepository repository)
            : base(repository)
        {
        }
    }
}