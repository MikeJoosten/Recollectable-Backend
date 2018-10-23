using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtAuthentication _jwtAuthentication;

        public AuthController(UserManager<User> userManager, 
            IOptions<JwtAuthentication> jwtAuthentication)
        {
            _userManager = userManager;
            _jwtAuthentication = jwtAuthentication?.Value;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] CredentialsDto credentials)
        {
            if (credentials == null)
            {
                return BadRequest();
            }

            var identity = GenerateClaimsIdentity(credentials.UserName, credentials.Password).Result;

            if (identity == null)
            {
                ModelState.AddModelError("Error", "Invalid username or password");
                return BadRequest(ModelState);
            }

            var response = new
            {
                userName = credentials.UserName,
                auth_token = GenerateEncodedToken(credentials.UserName, identity).Result,
                expires_in = (int)_jwtAuthentication.ValidFor.TotalSeconds
            };

            HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(identity));
            return Ok(response);
        }

        private async Task<ClaimsIdentity> GenerateClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            var user = await _userManager.FindByNameAsync(userName);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var identity = new ClaimsIdentity("Identity.Application");
                return await Task.FromResult(identity);
            }

            return await Task.FromResult<ClaimsIdentity>(null);
        }

        private async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtAuthentication.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, _jwtAuthentication.IssuedAt.ToString(), ClaimValueTypes.Integer64),
            };

            var token = new JwtSecurityToken(
                issuer: _jwtAuthentication.Issuer,
                audience: _jwtAuthentication.Audience,
                claims: claims,
                notBefore: _jwtAuthentication.NotBefore,
                expires: _jwtAuthentication.Expiration,
                signingCredentials: _jwtAuthentication.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}