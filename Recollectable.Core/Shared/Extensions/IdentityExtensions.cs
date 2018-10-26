using Microsoft.AspNetCore.Identity;
using Recollectable.Core.Entities.Users;
using System.Threading.Tasks;

namespace Recollectable.Core.Shared.Extensions
{
    public static class IdentityExtensions
    {
        public static async Task<User> FindByNameOrEmailAsync(this UserManager<User> userManager, string userName)
        {
            if (userName.Contains("@"))
            {
                var user = await userManager.FindByEmailAsync(userName);

                if (user != null)
                {
                    userName = user.UserName;
                }
            }

            return await userManager.FindByNameAsync(userName);
        }
    }
}