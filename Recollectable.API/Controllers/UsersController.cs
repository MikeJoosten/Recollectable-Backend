using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.Data.Helpers;
using Recollectable.Data.Repositories;
using Recollectable.Data.Services;
using Recollectable.Domain.Entities;
using Recollectable.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public UsersController(IUserRepository userRepository, 
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _userRepository = userRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers(UsersResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<UserDto, User>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<UserDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var usersFromRepo = _userRepository.GetUsers(resourceParameters);
            var users = Mapper.Map<IEnumerable<UserDto>>(usersFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = usersFromRepo.TotalCount,
                    pageSize = usersFromRepo.PageSize,
                    currentPage = usersFromRepo.CurrentPage,
                    totalPages = usersFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateUsersLinks(resourceParameters, 
                    usersFromRepo.HasNext, usersFromRepo.HasPrevious);
                var shapedUsers = users.ShapeData(resourceParameters.Fields);

                var linkedUsers = shapedUsers.Select(user =>
                {
                    var userAsDictionary = user as IDictionary<string, object>;
                    var userLinks = CreateUserLinks((Guid)userAsDictionary["Id"],
                        resourceParameters.Fields);

                    userAsDictionary.Add("links", userLinks);

                    return userAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = linkedUsers,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
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
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(users.ShapeData(resourceParameters.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetUser(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<UserDto>(fields))
            {
                return BadRequest();
            }

            var userFromRepo = _userRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = Mapper.Map<UserDto>(userFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateUserLinks(id, fields);
                var linkedResource = user.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(user.ShapeData(fields));
            }
        }

        [HttpPost(Name = "CreateUser")]
        public IActionResult CreateUser([FromBody] UserCreationDto user,
            [FromHeader(Name = "Accept")] string mediaType)
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

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateUserLinks(returnedUser.Id, null);
                var linkedResource = returnedUser.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetUser", new { id = returnedUser.Id }, linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetUser", new { id = returnedUser.Id }, returnedUser);
            }
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

        [HttpPut("{id}", Name = "UpdateUser")]
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

        [HttpPatch("{id}", Name = "PartiallyUpdateUser")]
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

        [HttpDelete("{id}", Name = "DeleteUser")]
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
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetUsers", new
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

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(_urlHelper.Link("GetUser",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("CreateUser", 
                    new { }), "create_user", "POST"));

                links.Add(new LinkDto(_urlHelper.Link("UpdateUser",
                    new { id }), "update_user", "PUT"));

                links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateUser",
                    new { id }), "partially_update_user", "PATCH"));

                links.Add(new LinkDto(_urlHelper.Link("DeleteUser",
                    new { id }), "delete_user", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateUsersLinks
            (UsersResourceParameters resourceParameters, 
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateUsersResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET"));

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