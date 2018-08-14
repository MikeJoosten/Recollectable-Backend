using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class CollectableUpdateDto : CollectableManipulationDto
    {
        [Required(ErrorMessage = "CollectionId is a required field")]
        [MaxLength(32, ErrorMessage = "CollectionId shouldn't contain more than 32 characters")]
        public Guid CollectionId { get; set; }
    }
}