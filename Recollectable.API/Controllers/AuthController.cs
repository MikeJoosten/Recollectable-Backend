using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenFactory _tokenFactory;

        public AuthController(UserManager<User> userManager, ITokenFactory tokenFactory)
        {
            _userManager = userManager;
            _tokenFactory = tokenFactory;
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
                auth_token = _tokenFactory.GenerateToken(credentials.UserName).Result,
                expires_in = (int)TokenProviderOptions.Expiration.TotalSeconds
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
    }
}