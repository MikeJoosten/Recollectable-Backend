namespace Recollectable.Core.Models.Users
{
    public abstract class UserManipulationDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}