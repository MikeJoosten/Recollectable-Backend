using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Users
{
    public class ChangedPasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}