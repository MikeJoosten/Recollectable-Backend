namespace Recollectable.Core.Models.Users
{
    public class ChangedPasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}