using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Models.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Helpers;
using Recollectable.Core.Shared.Models;
using Recollectable.Core.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    //TODO Add Authorization
    [ApiVersion("1.0")]
    [Route("api/collections")]
    public class CollectionsController : Controller
    {
        private ICollectionService _collectionService;
        private IUserService _userService;
        private IMapper _mapper;

        public CollectionsController(ICollectionService collectionService, 
            IUserService userService, IMapper mapper)
        {
            _collectionService = collectionService;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves collections
        /// </summary>
        /// <returns>List of collections</returns>
        /// <response code="200">Returns a list of collections</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetCollections")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CollectionDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCollections(CollectionsResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<Collection>(resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<CollectionDto>(resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedCollections = await _collectionService.FindCollections(resourceParameters);
            var collections = _mapper.Map<IEnumerable<CollectionDto>>(retrievedCollections);
            var shapedCollections = collections.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(resourceParameters.Fields) && !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var paginationMetadata = new
                {
                    totalCount = retrievedCollections.TotalCount,
                    pageSize = retrievedCollections.PageSize,
                    currentPage = retrievedCollections.CurrentPage,
                    totalPages = retrievedCollections.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCollectionsLinks(resourceParameters,
                    retrievedCollections.HasNext, retrievedCollections.HasPrevious);

                var linkedCollections = shapedCollections.Select(collection =>
                {
                    var collectionAsDictionary = collection as IDictionary<string, object>;
                    var collectionLinks = CreateCollectionLinks((Guid)collectionAsDictionary["Id"],
                        resourceParameters.Fields);

                    collectionAsDictionary.Add("links", collectionLinks);

                    return collectionAsDictionary;
                });

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedCollections,
                    Links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = retrievedCollections.HasPrevious ?
                    CreateCollectionsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedCollections.HasNext ?
                    CreateCollectionsResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedCollections.TotalCount,
                    pageSize = retrievedCollections.PageSize,
                    currentPage = retrievedCollections.CurrentPage,
                    totalPages = retrievedCollections.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(shapedCollections);
            }
        }

        /// <summary>
        /// Retrieves the requested collection by collection ID
        /// </summary>
        /// <param name="id">Collection ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested collection</returns>
        /// <response code="200">Returns the requested collection</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting collection ID</response>
        [HttpGet("{id}", Name = "GetCollection")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CollectionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCollection(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!TypeHelper.TypeHasProperties<CollectionDto>(fields))
            {
                return BadRequest();
            }

            var retrievedCollection = await _collectionService.FindCollectionById(id);

            if (retrievedCollection == null)
            {
                return NotFound();
            }

            var collection = _mapper.Map<CollectionDto>(retrievedCollection);
            var shapedCollection = collection.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateCollectionLinks(id, fields);
                var linkedResource = shapedCollection as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedCollection);
            }
        }

        //TODO Add Sample request
        /// <summary>
        /// Creates a collection
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /collections
        ///     {
        ///         
        ///     }
        /// </remarks>
        /// <param name="collection">Custom collection</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created collection</returns>
        /// <response code="201">Returns the newly created collection</response>
        /// <response code="400">Invalid collection</response>
        /// <response code="422">Invalid collection validation</response>
        [HttpPost(Name = "CreateCollection")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CollectionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCollection([FromBody] CollectionCreationDto collection,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (collection == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var user = await _userService.FindUserById(collection.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            var newCollection = _mapper.Map<Collection>(collection);
            newCollection.User = user;

            await _collectionService.CreateCollection(newCollection);

            if (!await _collectionService.Save())
            {
                throw new Exception("Creating a collection failed on save.");
            }

            var returnedCollection = _mapper.Map<CollectionDto>(newCollection);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectionLinks(returnedCollection.Id, null);
                var linkedResource = returnedCollection.ShapeData(null) as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCollection",
                    new { id = returnedCollection.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCollection",
                    new { id = returnedCollection.Id },
                    returnedCollection);
            }
        }

        /// <summary>
        /// Invalid collection creation request
        /// </summary>
        /// <param name="id">Collection ID</param>
        /// <response code="404">Unexisting collection ID</response>
        /// <response code="409">Already existing collection ID</response>
        [HttpPost("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockCollectionCreation(Guid id)
        {
            if (await _collectionService.CollectionExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        //TODO Add Sample request
        /// <summary>
        /// Updates a collection
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /collections/{id}
        ///     {
        ///         
        ///     }
        /// </remarks>
        /// <param name="id">Collection ID</param>
        /// <param name="collection">Custom collection</param>
        /// <response code="204">Updated the collection successfully</response>
        /// <response code="400">Invalid collection</response>
        /// <response code="404">Unexisting collection ID</response>
        /// <response code="422">Invalid collection validation</response>
        [HttpPut("{id}", Name = "UpdateCollection")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCollection(Guid id, [FromBody] CollectionUpdateDto collection)
        {
            if (collection == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _userService.UserExists(collection.UserId))
            {
                return BadRequest();
            }

            var retrievedCollection = await _collectionService.FindCollectionById(id);

            if (retrievedCollection == null)
            {
                return NotFound();
            }

            retrievedCollection.UserId = collection.UserId;

            _mapper.Map(collection, retrievedCollection);
            _collectionService.UpdateCollection(retrievedCollection);

            if (!await _collectionService.Save())
            {
                throw new Exception($"Updating collection {id} failed on save.");
            }

            return NoContent();
        }

        //TODO Add Sample request
        /// <summary>
        /// Update specific fields of a collection
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /collections/{id}
        ///     [
        ///	        
        ///     ]
        /// </remarks>
        /// <param name="id">Collection ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        /// <response code="204">Updated the collection successfully</response>
        /// <response code="400">Invalid patch document</response>
        /// <response code="404">Unexisting collection ID</response>
        /// <response code="422">Invalid collection validation</response>
        [HttpPatch("{id}", Name = "PartiallyUpdateCollection")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PartiallyUpdateCollection(Guid id,
            [FromBody] JsonPatchDocument<CollectionUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var retrievedCollection = await _collectionService.FindCollectionById(id);

            if (retrievedCollection == null)
            {
                return NotFound();
            }

            var patchedCollection = _mapper.Map<CollectionUpdateDto>(retrievedCollection);
            patchDoc.ApplyTo(patchedCollection, ModelState);

            TryValidateModel(patchedCollection);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _userService.UserExists(patchedCollection.UserId))
            {
                return BadRequest();
            }

            retrievedCollection.UserId = patchedCollection.UserId;

            _mapper.Map(patchedCollection, retrievedCollection);
            _collectionService.UpdateCollection(retrievedCollection);

            if (!await _collectionService.Save())
            {
                throw new Exception($"Patching collection {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Removes a collection
        /// </summary>
        /// <param name="id">Collection ID</param>
        /// <response code="204">Removed the collection successfully</response>
        /// <response code="404">Unexisting collection ID</response>
        [HttpDelete("{id}", Name = "DeleteCollection")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCollection(Guid id)
        {
            var retrievedCollection = await _collectionService.FindCollectionById(id);

            if (retrievedCollection == null)
            {
                return NotFound();
            }

            _collectionService.RemoveCollection(retrievedCollection);

            if (!await _collectionService.Save())
            {
                throw new Exception($"Deleting collection {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCollectionsOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateCollectionsResourceUri(CollectionsResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCollections", new
                    {
                        type = resourceParameters.Type,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCollections", new
                    {
                        type = resourceParameters.Type,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetCollections", new
                    {
                        type = resourceParameters.Type,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateCollectionLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("GetCollection",
                new { id }), "self", "GET"));

            links.Add(new LinkDto(Url.Link("CreateCollection",
                new { }), "create_collection", "POST"));

            links.Add(new LinkDto(Url.Link("UpdateCollection",
                new { id }), "update_collection", "PUT"));

            links.Add(new LinkDto(Url.Link("PartiallyUpdateCollection",
                new { id }), "partially_update_collection", "PATCH"));

            links.Add(new LinkDto(Url.Link("DeleteCollection",
                new { id }), "delete_collection", "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateCollectionsLinks
            (CollectionsResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateCollectionsResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCollectionsResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCollectionsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}