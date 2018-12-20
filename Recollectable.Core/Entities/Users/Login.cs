using System.Collections.Generic;

namespace Recollectable.Core.Entities.Users
{
    public class Login
    {
        public string UserName { get; set; }
        public string AuthToken { get; set; }
        public double ExpiresIn { get; set; }
        public IList<string> Roles { get; set; }
    }
}