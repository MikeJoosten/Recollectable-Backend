using System;

namespace Recollectable.API.Models.Collections
{
    public abstract class CollectionCollectableManipulationDto
    {
        public Guid CollectableId { get; set; }
        public Guid ConditionId { get; set; }
    }
}