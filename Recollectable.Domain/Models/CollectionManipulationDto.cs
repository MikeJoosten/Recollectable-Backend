using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Models
{
    public abstract class CollectionManipulationDto
    {
        [Required(ErrorMessage = "Type is a required field")]
        [MaxLength(25, ErrorMessage = "Type shouldn't contain more than 25 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "UserId is a required field")]
        public Guid UserId { get; set; }
    }
}