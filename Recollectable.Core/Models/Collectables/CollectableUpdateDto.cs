using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Collectables
{
    public class CollectableUpdateDto : CollectableManipulationDto
    {
        public Guid CollectionId { get; set; }
    }
}