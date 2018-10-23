using Recollectable.Core.Entities.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Users
{
    public class UserCreationDto : UserManipulationDto
    {
        [Required(ErrorMessage = "Username is a required field")]
        [MaxLength(50, ErrorMessage = "Password shouldn't contain more than 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        public string Password { get; set; }
    }
}