using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Domain.Models
{
    public abstract class CollectableManipulationDto
    {
        [Required(ErrorMessage = "CollectableId is a required field")]
        public Guid CollectableId { get; set; }

        [Required(ErrorMessage = "ConditionId is a required field")]
        public Guid ConditionId { get; set; }
    }
}