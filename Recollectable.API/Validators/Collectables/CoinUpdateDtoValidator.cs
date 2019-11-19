using AutoMapper;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Interfaces;

namespace Recollectable.API.Validators.Collectables
{
    public class CoinUpdateDtoValidator : CoinManipulationDtoValidator<CoinUpdateDto>
    {
        public CoinUpdateDtoValidator(ICoinService service, IMapper mapper)
            : base(service, mapper)
        {
        }
    }
}