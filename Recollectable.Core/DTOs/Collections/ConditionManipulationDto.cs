using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.DTOs.Collections
{
    public abstract class ConditionManipulationDto
    {
        [Required(ErrorMessage = "Grade is a required field")]
        [MaxLength(50, ErrorMessage = "Grade shouldn't contain more than 50 characters")]
        public string Grade { get; set; }
    }
}