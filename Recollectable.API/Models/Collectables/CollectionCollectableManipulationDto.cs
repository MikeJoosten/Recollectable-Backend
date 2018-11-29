using System;

namespace Recollectable.API.Models.Collectables
{
    public abstract class CollectionCollectableManipulationDto
    {
        public Guid CollectableId { get; set; }
        public string Condition { get; set; }
    }
}