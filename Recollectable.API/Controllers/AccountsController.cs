using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private UserManager<User> _userManager;
        private IMapper _mapper;

        public AccountsController(IUnitOfWork unitOfWork, ITypeHelperService typeHelperService,
            IPropertyMappingService propertyMappingService, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetAccounts")]
        public IActionResult GetAccounts(UsersResourceParameters resourceParameters,
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

            var usersFromRepo = _unitOfWork.UserRepository.Get(resourceParameters);
            var users = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);

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

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedUsers,
                    Links = links
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

        [HttpGet("{id}", Name = "GetAccount")]
        public IActionResult GetAccount(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<UserDto>(fields))
            {
                return BadRequest();
            }

            var userFromRepo = _unitOfWork.UserRepository.GetById(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(userFromRepo);

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

        [HttpPost(Name = "CreateAccount")]
        public IActionResult CreateAccount([FromBody] UserCreationDto user,
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
            var result = _userManager.CreateAsync(newUser, user.Password);

            if (!result.Result.Succeeded)
            {
                return new UnprocessableEntityObjectResult(result.Result);
            }

            if (!_unitOfWork.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            var returnedUser = _mapper.Map<UserDto>(newUser);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateUserLinks(returnedUser.Id, null);
                var linkedResource = returnedUser.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetAccount", new { id = returnedUser.Id }, linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetAccount", new { id = returnedUser.Id }, returnedUser);
            }
        }

        [HttpPost("{id}")]
        public IActionResult BlockAccountCreation(Guid id)
        {
            if (_unitOfWork.UserRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateAccount")]
        public IActionResult UpdateAccount(Guid id, [FromBody] UserUpdateDto user)
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

            _mapper.Map(user, userFromRepo);
            _unitOfWork.UserRepository.Update(userFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Updating user {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateAccount")]
        public IActionResult PartiallyUpdateAccount(Guid id,
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

            var patchedUser = _mapper.Map<UserUpdateDto>(userFromRepo);
            patchDoc.ApplyTo(patchedUser, ModelState);

            TryValidateModel(patchedUser);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(patchedUser, userFromRepo);
            _unitOfWork.UserRepository.Update(userFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching user {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteAccount")]
        public IActionResult DeleteAccount(Guid id)
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
                    return Url.Link("GetAccounts", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAccounts", new
                    {
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetAccounts", new
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
                links.Add(new LinkDto(Url.Link("GetAccount",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(Url.Link("CreateAccount",
                    new { }), "create_account", "POST"));

                links.Add(new LinkDto(Url.Link("UpdateAccount",
                    new { id }), "update_account", "PUT"));

                links.Add(new LinkDto(Url.Link("PartiallyUpdateAccount",
                    new { id }), "partially_update_account", "PATCH"));

                links.Add(new LinkDto(Url.Link("DeleteAccount",
                    new { id }), "delete_account", "DELETE"));
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