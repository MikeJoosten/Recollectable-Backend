using Microsoft.Extensions.Options;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Helpers;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Recollectable.Core.Shared.Factories
{
    public class TokenFactory : ITokenFactory
    {
        private readonly TokenProviderOptions _tokenProviderOptions;

        public TokenFactory(IOptions<TokenProviderOptions> tokenProviderOptions)
        {
            _tokenProviderOptions = tokenProviderOptions?.Value;
        }

        public async Task<string> GenerateToken(string userName)
        {
            DateTime now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: _tokenProviderOptions.Issuer,
                audience: _tokenProviderOptions.Audience,
                claims: GetTokenClaims(userName),
                notBefore: now,
                expires: now.Add(TokenProviderOptions.Expiration),
                signingCredentials: _tokenProviderOptions.SigningCredentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(encodedToken);
        }

        private Claim[] GetTokenClaims(string userName)
        {
            return new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateHelper.ToUnixEpochDate(_tokenProviderOptions.IssuedAt).ToString(),
                    ClaimValueTypes.Integer64)
            };
        }
    }
}