using Microsoft.IdentityModel.Tokens;
using System;

namespace Recollectable.Core.Shared.Entities
{
    public class TokenProviderOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public DateTime IssuedAt => DateTime.UtcNow;
        public static TimeSpan Expiration { get; set; } = TimeSpan.FromDays(1);

        public SymmetricSecurityKey SecurityKey =>
            new SymmetricSecurityKey(Convert.FromBase64String(SecretKey));

        public SigningCredentials SigningCredentials =>
            new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }
}