using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.DTOs.Locations
{
    public abstract class CountryManipulationDto
    {
        [Required(ErrorMessage = "Name is a required field")]
        [MaxLength(50, ErrorMessage = "Name shouldn't contain more than 50 characters")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}