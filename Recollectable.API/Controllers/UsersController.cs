using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Helpers;
using Recollectable.API.Models;
using Recollectable.Data.Helpers;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;
        private IUrlHelper _urlHelper;

        public UsersController(IUserRepository userRepository, IUrlHelper urlHelper)
        {
            _userRepository = userRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers(UsersResourceParameters resourceParameters)
        {
            var usersFromRepo = _userRepository.GetUsers(resourceParameters);

            var previousPageLink = usersFromRepo.HasPrevious ?
                CreateUsersResourceUri(resourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = usersFromRepo.HasNext ?
                CreateUsersResourceUri(resourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = usersFromRepo.TotalCount,
                pageSize = usersFromRepo.PageSize,
                currentPage = usersFromRepo.CurrentPage,
                totalPages = usersFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var users = Mapper.Map<IEnumerable<UserDto>>(usersFromRepo);
            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetUser(Guid id)
        {
            var userFromRepo = _userRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = Mapper.Map<UserDto>(userFromRepo);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserCreationDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var newUser = Mapper.Map<User>(user);
            _userRepository.AddUser(newUser);

            if (!_userRepository.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            var returnedUser = Mapper.Map<UserDto>(newUser);
            return CreatedAtRoute("GetUser", new { id = returnedUser.Id }, returnedUser);
        }

        [HttpPost("{id}")]
        public IActionResult BlockUserCreation(Guid id)
        {
            if (_userRepository.UserExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, [FromBody] UserUpdateDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userFromRepo = _userRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(user, userFromRepo);
            _userRepository.UpdateUser(userFromRepo);

            if (!_userRepository.Save())
            {
                throw new Exception($"Updating user {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateUser(Guid id,
            [FromBody] JsonPatchDocument<UserUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var userFromRepo = _userRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var patchedUser = Mapper.Map<UserUpdateDto>(userFromRepo);
            patchDoc.ApplyTo(patchedUser);

            Mapper.Map(patchedUser, userFromRepo);
            _userRepository.UpdateUser(userFromRepo);

            if (!_userRepository.Save())
            {
                throw new Exception($"Patching user {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            var userFromRepo = _userRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            _userRepository.DeleteUser(userFromRepo);

            if (!_userRepository.Save())
            {
                throw new Exception($"Deleting user {id} failed on save.");
            }

            return NoContent();
        }

        private string CreateUsersResourceUri(UsersResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }
    }
}