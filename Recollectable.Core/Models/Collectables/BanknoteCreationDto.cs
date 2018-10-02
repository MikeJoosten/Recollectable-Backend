using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Locations;

namespace Recollectable.Core.Models.Collectables
{
    public class BanknoteCreationDto : BanknoteManipulationDto
    {
        public Country Country { get; set; }
        public CollectorValue CollectorValue { get; set; }
    }
}