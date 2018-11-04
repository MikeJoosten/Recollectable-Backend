using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Collectables
{
    public abstract class CollectableManipulationDto
    {
        public Guid CollectableId { get; set; }
        public string Condition { get; set; }
    }
}