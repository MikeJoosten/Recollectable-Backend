using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Collections
{
    public abstract class CollectionManipulationDto
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}