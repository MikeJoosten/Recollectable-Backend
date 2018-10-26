using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Users
{
    public class UserCreationDto : UserManipulationDto
    {
        [Required(ErrorMessage = "Username is a required field")]
        [MaxLength(50, ErrorMessage = "Username shouldn't contain more than 50 characters")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters allowed")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        public string Password { get; set; }
    }
}