using System;

namespace Recollectable.Core.Models.Collectables
{
    public class CollectionCollectableUpdateDto : CollectionCollectableManipulationDto
    {
        public Guid CollectionId { get; set; }
    }
}