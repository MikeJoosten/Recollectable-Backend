using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Shared.Entities
{
    public class JwtAuthentication
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(120);
        public DateTime Expiration => IssuedAt.Add(ValidFor);

        public Func<Task<string>> JtiGenerator =>
            () => Task.FromResult(Guid.NewGuid().ToString());

        public SymmetricSecurityKey SecurityKey => 
            new SymmetricSecurityKey(Convert.FromBase64String(SecretKey));

        public SigningCredentials SigningCredentials =>
            new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }
}