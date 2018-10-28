using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Collectables
{
    public abstract class CollectableManipulationDto
    {
        [Required(ErrorMessage = "CollectableId is a required field")]
        public Guid CollectableId { get; set; }

        [Required(ErrorMessage = "Condition is a required field")]
        public string Condition { get; set; }

        [Required(ErrorMessage = "Count is a required field")]
        public int Count { get; set; }
    }
}