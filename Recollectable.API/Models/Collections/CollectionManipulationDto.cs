using System;

namespace Recollectable.API.Models.Collections
{
    public abstract class CollectionManipulationDto
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}