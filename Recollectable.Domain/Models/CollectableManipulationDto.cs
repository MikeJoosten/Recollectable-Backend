using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Domain.Models
{
    public abstract class CollectableManipulationDto
    {
        [Required(ErrorMessage = "CollectableId is a required field")]
        [MaxLength(32, ErrorMessage = "CollectableId shouldn't contain more than 32 characters")]
        public Guid CollectableId { get; set; }

        [Required(ErrorMessage = "ConditionId is a required field")]
        [MaxLength(32, ErrorMessage = "ConditionId shouldn't contain more than 32 characters")]
        public Guid ConditionId { get; set; }
    }
}