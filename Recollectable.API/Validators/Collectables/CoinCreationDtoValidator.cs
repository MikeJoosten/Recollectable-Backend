using AutoMapper;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Interfaces;

namespace Recollectable.API.Validators.Collectables
{
    public class CoinCreationDtoValidator : CoinManipulationDtoValidator<CoinCreationDto>
    {
        public CoinCreationDtoValidator(ICoinService service, IMapper mapper)
            : base(service, mapper)
        {
        }
    }
}