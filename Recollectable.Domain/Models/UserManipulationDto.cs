using System.ComponentModel.DataAnnotations;

namespace Recollectable.Domain.Models
{
    public abstract class UserManipulationDto
    {
        [Required(ErrorMessage = "First name is a required field")]
        [MaxLength(100, ErrorMessage = "First name shouldn't contain more than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is a required field")]
        [MaxLength(100, ErrorMessage = "Last name shouldn't contain more than 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is a required field")]
        [EmailAddress(ErrorMessage = "An invalid email address has been entered")]
        [MaxLength(250, ErrorMessage = "Email shouldn't contain more than 250 characters")]
        public string Email { get; set; }
    }
}