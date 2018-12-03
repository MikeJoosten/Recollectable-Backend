using System;

namespace Recollectable.API.Models.Collections
{
    public class CollectionCollectableUpdateDto : CollectionCollectableManipulationDto
    {
        public Guid CollectionId { get; set; }
    }
}