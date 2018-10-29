using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Collectables
{
    public abstract class CollectableManipulationDto
    {
        [Required(ErrorMessage = "CollectableId is a required field")]
        public Guid CollectableId { get; set; }

        [Required(ErrorMessage = "Condition is a required field")]
        [MaxLength(50, ErrorMessage = "Condition shouldn't contain more than 50 characters")]
        public string Condition { get; set; }
    }
}