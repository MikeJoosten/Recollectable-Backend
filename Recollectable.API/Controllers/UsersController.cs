using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IControllerService _controllerService;

        public UsersController(IUnitOfWork unitOfWork,
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers(UsersResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<UserDto, User>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<UserDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var usersFromRepo = _unitOfWork.UserRepository.Get(resourceParameters);
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
            else if (mediaType == "application/json")
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
            else
            {
                return Ok(users);
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetUser(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.TypeHelperService.TypeHasProperties<UserDto>(fields))
            {
                return BadRequest();
            }

            var userFromRepo = _unitOfWork.UserRepository.GetById(id);

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
            else if (mediaType == "application/json")
            {
                return Ok(user.ShapeData(fields));
            }
            else
            {
                return Ok(user);
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

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var newUser = Mapper.Map<User>(user);
            _unitOfWork.UserRepository.Add(newUser);

            if (!_unitOfWork.Save())
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
            if (_unitOfWork.UserRepository.Exists(id))
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

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var userFromRepo = _unitOfWork.UserRepository.GetById(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(user, userFromRepo);
            _unitOfWork.UserRepository.Update(userFromRepo);

            if (!_unitOfWork.Save())
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

            var userFromRepo = _unitOfWork.UserRepository.GetById(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var patchedUser = Mapper.Map<UserUpdateDto>(userFromRepo);
            patchDoc.ApplyTo(patchedUser, ModelState);

            TryValidateModel(patchedUser);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(patchedUser, userFromRepo);
            _unitOfWork.UserRepository.Update(userFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching user {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        public IActionResult DeleteUser(Guid id)
        {
            var userFromRepo = _unitOfWork.UserRepository.GetById(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.UserRepository.Delete(userFromRepo);

            if (!_unitOfWork.Save())
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

        private string CreateUsersResourceUri(UsersResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _controllerService.UrlHelper.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _controllerService.UrlHelper.Link("GetUsers", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.Current:
                default:
                    return _controllerService.UrlHelper.Link("GetUsers", new
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
                links.Add(new LinkDto(_controllerService.UrlHelper.Link("GetUser",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("CreateUser", 
                    new { }), "create_user", "POST"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("UpdateUser",
                    new { id }), "update_user", "PUT"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("PartiallyUpdateUser",
                    new { id }), "partially_update_user", "PATCH"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("DeleteUser",
                    new { id }), "delete_user", "DELETE"));
            }

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