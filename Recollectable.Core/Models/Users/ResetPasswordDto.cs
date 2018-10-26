using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Models.Users
{
    public class ResetPasswordDto
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}