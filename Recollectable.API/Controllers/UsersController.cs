using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.API.Models.Users;
using Recollectable.API.ViewModels;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Helpers;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Core.Shared.Models;
using Recollectable.Core.Shared.Services;
using Recollectable.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/users")]
    //TODO Add Authorization [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private IUserService _userService;
        private UserManager<User> _userManager;
        private ITokenFactory _tokenFactory;
        private IEmailService _emailService;
        private IMapper _mapper;
        private IRazorViewToStringRenderer _razorViewRenderer;

        public UsersController(IUserService userService, UserManager<User> userManager, 
            ITokenFactory tokenFactory, IEmailService emailService, IMapper mapper,
            IRazorViewToStringRenderer razorViewRenderer)
        {
            _userService = userService;
            _userManager = userManager;
            _tokenFactory = tokenFactory;
            _emailService = emailService;
            _mapper = mapper;
            _razorViewRenderer = razorViewRenderer;
        }

        /// <summary>
        /// Retrieves users
        /// </summary>
        /// <returns>List of users</returns>
        /// <response code="200">Returns a list of users</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetUsers")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUsers(UsersResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<User>(resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<UserDto>(resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedUsers = await _userService.FindUsers(resourceParameters);
            var users = _mapper.Map<IEnumerable<UserDto>>(retrievedUsers);
            var shapedUsers = users.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(resourceParameters.Fields) &&
                    !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var paginationMetadata = new
                {
                    totalCount = retrievedUsers.TotalCount,
                    pageSize = retrievedUsers.PageSize,
                    currentPage = retrievedUsers.CurrentPage,
                    totalPages = retrievedUsers.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateUsersLinks(resourceParameters,
                    retrievedUsers.HasNext, retrievedUsers.HasPrevious);

                var linkedUsers = shapedUsers.Select(user =>
                {
                    var userAsDictionary = user as IDictionary<string, object>;
                    var userLinks = CreateUserLinks((Guid)userAsDictionary["Id"],
                        resourceParameters.Fields);

                    userAsDictionary.Add("links", userLinks);

                    return userAsDictionary;
                });

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedUsers,
                    Links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = retrievedUsers.HasPrevious ?
                    CreateUsersResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedUsers.HasNext ?
                    CreateUsersResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedUsers.TotalCount,
                    pageSize = retrievedUsers.PageSize,
                    currentPage = retrievedUsers.CurrentPage,
                    totalPages = retrievedUsers.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(shapedUsers);
            }
        }

        /// <summary>
        /// Retrieves the requested user by user ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested user</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting user ID</response>
        [HttpGet("{id}", Name = "GetUser")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!TypeHelper.TypeHasProperties<UserDto>(fields))
            {
                return BadRequest();
            }

            var retrievedUser = await _userService.FindUserById(id);

            if (retrievedUser == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(retrievedUser);
            var shapedUser = user.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateUserLinks(id, fields);
                var linkedResource = shapedUser as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedUser);
            }
        }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /register
        ///     {
        ///         "userName": "Kara",
        ///         "firstName": "Kara",
        ///         "lastName": "Eberle",
        ///         "password": "password123",
        ///         "email": "kara.eberle@gmail.com"
        ///     }
        /// </remarks>
        /// <param name="user">Custom user</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created user</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">Invalid user</response>
        /// <response code="422">Invalid user validation</response>
        [AllowAnonymous]
        [HttpPost("register", Name = "Register")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Register([FromBody] UserCreationDto user,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var newUser = _mapper.Map<User>(user);
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                return new UnprocessableEntityObjectResult(result);
            }

            result = await _userManager.AddToRoleAsync(newUser, nameof(Roles.User));

            if (!result.Succeeded)
            {
                return new UnprocessableEntityObjectResult(result);
            }

            if (!await _userService.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationUrl = Url.Action("ConfirmEmail", "Users", new { token, email = newUser.Email },
                protocol: HttpContext.Request.Scheme);

            //TODO Activate Mailing Service + Edit appsettings.json
            //var confirmAccountModel = new AccountEmailViewModel(confirmationUrl, user.UserName);
            //var body = await _razorViewRenderer.RenderViewToStringAsync("/Views/ConfirmAccountEmail.cshtml", confirmAccountModel);
            //await _emailService.Send(user.Email, "Activate your account", body, MailType.Confirmation);

            var returnedUser = _mapper.Map<UserDto>(newUser);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateUserLinks(returnedUser.Id, null);
                var linkedResource = returnedUser.ShapeData(null) as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetUser", new { id = returnedUser.Id }, linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetUser", new { id = returnedUser.Id }, returnedUser);
            }
        }

        /// <summary>
        /// Invalid user creation request
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="404">Unexisting user ID</response>
        /// <response code="409">Already existing user ID</response>
        [HttpPost("register/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockRegistration(Guid id)
        {
            if (await _userService.UserExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        /// <summary>
        /// Retrieves login details of a user
        /// </summary>
        /// <param name="credentials">Login credentials</param>
        /// <returns>Returns login details with authentication token</returns>
        /// <response code="200">Returns the login details</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="404">Unexisting username</response>
        [AllowAnonymous]
        [HttpPost("login", Name = "Login")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(Login), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
        {
            if (credentials == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(credentials.UserName);
            user = user ?? await _userManager.FindByEmailAsync(credentials.UserName);

            if (user == null)
            {
                return NotFound();
            }

            var identity = await GenerateClaimsIdentity(user, credentials.Password);

            if (identity == null)
            {
                return BadRequest(ModelState);
            }

            var response = new Login
            {
                UserName = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result,
                AuthToken = _tokenFactory.GenerateToken(credentials.UserName, identity).Result,
                ExpiresIn = (int)JwtTokenProviderOptions.Expiration.TotalSeconds
            };

            await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(identity));
            return Ok(response);
        }

        /// <summary>
        /// Request a password reset
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <response code="204">Sent password reset mail successfully</response>
        /// <response code="404">Unexisting email address</response>
        [AllowAnonymous]
        [HttpGet("{email}/forgot_password")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = Url.Action("ResetPassword", "Users", new { token, email },
                protocol: HttpContext.Request.Scheme);

            //TODO Activate Mailing Service + Edit appsettings.json - Different link
            //var resetPasswordModel = new AccountEmailViewModel(resetUrl, null);
            //var body = await _razorViewRenderer.RenderViewToStringAsync("/Views/ResetPasswordEmail.cshtml", resetPasswordModel);
            //await _emailService.Send(email, "Reset password", body, MailType.Reset);

            return NoContent();
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="token">Password reset token</param>
        /// <param name="email">User's email address</param>
        /// <param name="resetPassword">Reset and confirmation password</param>
        /// <response code="204">Password reset successfully</response>
        /// <response code="400">Invalid password</response>
        /// <response code="404">Unexisting email address</response>
        /// <response code="422">Invalid password validation</response>
        [AllowAnonymous]
        [HttpPost("/{email}/reset_password")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> ResetPassword(string token, string email, [FromBody] ResetPasswordDto resetPassword)
        {
            if (resetPassword == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ResetPasswordAsync(user, token, resetPassword.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
                return BadRequest(ModelState);
            }

            if (_userManager.IsLockedOutAsync(user).Result)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            }

            return NoContent();
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="changedPassword">Necessary passwords</param>
        /// <response code="204">Password changed successfully</response>
        /// <response code="400">Invalid password</response>
        /// <response code="404">Unexisting email address</response>
        /// <response code="422">Invalid password validation</response>
        [Authorize(Roles = "User")]
        [HttpPost("/{email}/change_password")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> ChangePassword(string email, [FromBody] ChangedPasswordDto changedPassword)
        {
            if (changedPassword == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, changedPassword.OldPassword, changedPassword.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Verifies a user's email address
        /// </summary>
        /// <param name="token">Email verification token</param>
        /// <param name="email">User's email address</param>
        /// <response code="204">Verified user's email address successfully</response>
        /// <response code="404">Unexisting email address</response>
        [AllowAnonymous]
        [HttpGet("{email}/confirm_account")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            //TODO Activate Mailing Service + Edit appsettings.json
            //var validatedAccountModel = new AccountEmailViewModel(null, user.UserName);
            //var body = await _razorViewRenderer.RenderViewToStringAsync("/Views/WelcomeAccountEmail.cshtml", validatedAccountModel);
            //await _emailService.Send(email, "Welcome to Recollectable", body, MailType.Welcome);

            return NoContent();
        }

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /users/{id}
        ///     {
        ///         "userName": "Gavin",
        ///         "firstName": "Gavin",
        ///         "lastName": "Free",
        ///         "password": "password123",
        ///         "email": "gavin.free@gmail.com"
        ///     }
        /// </remarks>
        /// <param name="id">User ID</param>
        /// <param name="user">Custom user</param>
        /// <response code="204">Updated the user successfully</response>
        /// <response code="400">Invalid user</response>
        /// <response code="404">Unexisting user ID</response>
        /// <response code="422">Invalid user validation</response>
        [HttpPut("{id}", Name = "UpdateUser")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var retrievedUser = await _userService.FindUserById(id);

            if (retrievedUser == null)
            {
                return NotFound();
            }

            _mapper.Map(user, retrievedUser);
            _userService.UpdateUser(retrievedUser);

            if (!await _userService.Save())
            {
                throw new Exception($"Updating user {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Update specific fields of a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /users/{id}
        ///     [
        ///	        { "op": "replace", "path": "/userName", "value": "Alfredo" },
        ///	        { "op": "copy", "from": "/firstName", "path": "/userName" },
        ///	        { "op": "move", "from": "/lastName", "path": "/userName" },
        ///	        { "op": "remove", "path": "/userName" }  
        ///     ]
        /// </remarks>
        /// <param name="id">User ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        /// <response code="204">Updated the user successfully</response>
        /// <response code="400">Invalid patch document</response>
        /// <response code="404">Unexisting user ID</response>
        /// <response code="422">Invalid user validation</response>
        [HttpPatch("{id}", Name = "PartiallyUpdateUser")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PartiallyUpdateUser(Guid id,
            [FromBody] JsonPatchDocument<UserUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var retrievedUser = await _userService.FindUserById(id);

            if (retrievedUser == null)
            {
                return NotFound();
            }

            var patchedUser = _mapper.Map<UserUpdateDto>(retrievedUser);
            patchDoc.ApplyTo(patchedUser, ModelState);

            TryValidateModel(patchedUser);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(patchedUser, retrievedUser);
            _userService.UpdateUser(retrievedUser);

            if (!await _userService.Save())
            {
                throw new Exception($"Patching user {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Removes a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="204">Removed the user successfully</response>
        /// <response code="404">Unexisting user ID</response>
        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var retrievedUser = await _userService.FindUserById(id);

            if (retrievedUser == null)
            {
                return NotFound();
            }

            _userService.RemoveUser(retrievedUser);

            if (!await _userService.Save())
            {
                throw new Exception($"Deleting user {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetUsersOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private async Task<ClaimsIdentity> GenerateClaimsIdentity(User user, string password)
        {
            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("Error", "Invalid username");
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Error", "Invalid password");
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            if (!await _userManager.IsLockedOutAsync(user))
            {
                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("Error", "Email is not confirmed");
                        return await Task.FromResult<ClaimsIdentity>(null);
                    }

                    await _userManager.ResetAccessFailedCountAsync(user);

                    var roles = await _userManager.GetRolesAsync(user);
                    var identity = new ClaimsIdentity("Identity.Application");
                    identity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    return await Task.FromResult(identity);
                }

                await _userManager.AccessFailedAsync(user);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("Error", "User locked out");
                //TODO Notify user + Send email password reset
            }
            else
            {
                ModelState.AddModelError("Error", "Invalid password");
            }

            return await Task.FromResult<ClaimsIdentity>(null);
        }

        private string CreateUsersResourceUri(UsersResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateUserLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("GetUser",
                new { id }), "self", "GET"));

            links.Add(new LinkDto(Url.Link("Register",
                new { }), "register_user", "POST"));

            links.Add(new LinkDto(Url.Link("Login",
                new { }), "login_user", "POST"));

            links.Add(new LinkDto(Url.Link("UpdateUser",
                new { id }), "update_user", "PUT"));

            links.Add(new LinkDto(Url.Link("PartiallyUpdateUser",
                new { id }), "partially_update_user", "PATCH"));

            links.Add(new LinkDto(Url.Link("DeleteUser",
                new { id }), "delete_user", "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateUsersLinks
            (UsersResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateUsersResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateUsersResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateUsersResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}