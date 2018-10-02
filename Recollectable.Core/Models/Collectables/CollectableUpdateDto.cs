using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Collectables
{
    public class CollectableUpdateDto : CollectableManipulationDto
    {
        [Required(ErrorMessage = "CollectionId is a required field")]
        [MaxLength(32, ErrorMessage = "CollectionId shouldn't contain more than 32 characters")]
        public Guid CollectionId { get; set; }
    }
}