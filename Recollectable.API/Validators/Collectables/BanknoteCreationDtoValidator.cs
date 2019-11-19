using AutoMapper;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Interfaces;
using System.Collections.Generic;

namespace Recollectable.API.Validators.Collectables
{
    public class BanknoteCreationDtoValidator : BanknoteManipulationDtoValidator<BanknoteCreationDto>
    {
        public BanknoteCreationDtoValidator(IBanknoteService service, IEqualityComparer<Currency> comparer, IMapper mapper)
            : base(service, comparer, mapper)
        {
        }
    }
}