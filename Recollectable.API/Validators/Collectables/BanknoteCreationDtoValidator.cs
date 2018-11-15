using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using System.Collections.Generic;

namespace Recollectable.API.Validators.Collectables
{
    public class BanknoteCreationDtoValidator : BanknoteManipulationDtoValidator<BanknoteCreationDto>
    {
        public BanknoteCreationDtoValidator(IUnitOfWork unitOfWork, IEqualityComparer<Currency> comparer)
            : base(unitOfWork, comparer)
        {
        }
    }
}