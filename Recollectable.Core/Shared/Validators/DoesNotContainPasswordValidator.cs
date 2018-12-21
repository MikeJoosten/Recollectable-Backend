using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Recollectable.Core.Shared.Validators
{
    public class DoesNotContainPasswordValidator<TUser> : IPasswordValidator<TUser>
        where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var userName = await manager.GetUserNameAsync(user);

            if (userName.ToLowerInvariant() == password.ToLowerInvariant())
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Password can't be equal to the username"
                });
            }

            if (password.ToLowerInvariant().Contains("password"))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Password can't contain 'password'"
                });
            }

            return IdentityResult.Success;
        }
    }
}