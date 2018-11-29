using System;

namespace Recollectable.API.Models.Collectables
{
    public class CollectionCollectableUpdateDto : CollectionCollectableManipulationDto
    {
        public Guid CollectionId { get; set; }
    }
}