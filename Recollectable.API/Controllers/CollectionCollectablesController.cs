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
    [Route("api/collections/{collectionId}/collectables")]
    public class CollectionCollectablesController : Controller
    {
        private ICollectionCollectableService _collectableService;
        private ICollectionService _collectionService;
        private IMapper _mapper;

        public CollectionCollectablesController(ICollectionCollectableService collectableService, 
            ICollectionService collectionService, IMapper mapper)
        {
            _collectableService = collectableService;
            _collectionService = collectionService;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectionCollectables")]
        public async Task<IActionResult> GetCollectionCollectables(Guid collectionId, 
            CollectionCollectablesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<CollectionCollectable>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<CollectionCollectableDto>(resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedCollectables = await _collectableService
                .FindCollectionCollectables(collectionId, resourceParameters);

            if (retrievedCollectables == null)
            {
                return NotFound();
            }

            var collectables = _mapper.Map<IEnumerable<CollectionCollectableDto>>(retrievedCollectables);
            var shapedCollectables = collectables.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(resourceParameters.Fields) && !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var paginationMetadata = new
                {
                    totalCount = retrievedCollectables.TotalCount,
                    pageSize = retrievedCollectables.PageSize,
                    currentPage = retrievedCollectables.CurrentPage,
                    totalPages = retrievedCollectables.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCollectionCollectablesLinks(resourceParameters,
                    retrievedCollectables.HasNext, retrievedCollectables.HasPrevious);

                var linkedCollectables = shapedCollectables.Select(collectable =>
                {
                    var collectableAsDictionary = collectable as IDictionary<string, object>;
                    var collectableLinks = CreateCollectionCollectableLinks((Guid)collectableAsDictionary["Id"],
                        resourceParameters.Fields);

                    collectableAsDictionary.Add("links", collectableLinks);

                    return collectableAsDictionary;
                });

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedCollectables,
                    Links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = retrievedCollectables.HasPrevious ?
                    CreateCollectionCollectablesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedCollectables.HasNext ?
                    CreateCollectionCollectablesResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedCollectables.TotalCount,
                    pageSize = retrievedCollectables.PageSize,
                    currentPage = retrievedCollectables.CurrentPage,
                    totalPages = retrievedCollectables.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(shapedCollectables);
            }
        }

        [HttpGet("{id}", Name = "GetCollectionCollectable")]
        public async Task<IActionResult> GetCollectionCollectable(Guid collectionId, Guid id, 
            [FromQuery] string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!TypeHelper.TypeHasProperties<CollectionCollectableDto>(fields))
            {
                return BadRequest();
            }

            var retrievedCollectable = await _collectableService.FindCollectionCollectableById(collectionId, id);

            if (retrievedCollectable == null)
            {
                return NotFound();
            }

            var collectable = _mapper.Map<CollectionCollectableDto>(retrievedCollectable);
            var shapedCollectable = collectable.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateCollectionCollectableLinks(id, fields);
                var linkedResource = shapedCollectable as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedCollectable);
            }
        }

        [HttpPost(Name = "CreateCollectionCollectable")]
        public async Task<IActionResult> CreateCollectionCollectable(Guid collectionId, 
            [FromBody] CollectionCollectableCreationDto collectable,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (collectable == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var collection = await _collectionService.FindCollectionById(collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var collectableItem = await _collectableService
                .FindCollectableById(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLowerInvariant().Contains(collection.Type.ToLowerInvariant()))
            {
                return BadRequest();
            }

            var newCollectable = _mapper.Map<CollectionCollectable>(collectable);
            newCollectable.CollectionId = collectionId;
            newCollectable.Collection = collection;
            newCollectable.Collectable = collectableItem;

            await _collectableService.CreateCollectionCollectable(newCollectable);

            if (!await _collectableService.Save())
            {
                throw new Exception("Creating a collectable failed on save.");
            }

            var returnedCollectable = _mapper.Map<CollectionCollectableDto>(newCollectable);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectionCollectableLinks(returnedCollectable.Id, null);
                var linkedResource = returnedCollectable.ShapeData(null) as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCollectionCollectable",
                    new { id = returnedCollectable.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCollectionCollectable",
                    new { id = returnedCollectable.Id },
                    returnedCollectable);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> BlockCollectionCollectableCreation(Guid collectionId, Guid id)
        {
            if (await _collectableService.CollectionCollectableExists(collectionId, id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCollectionCollectable")]
        public async Task<IActionResult> UpdateCollectionCollectable(Guid collectionId, Guid id,
            [FromBody] CollectionCollectableUpdateDto collectable)
        {
            if (collectable == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (collectable.CollectionId == Guid.Empty)
            {
                collectable.CollectionId = collectionId;
            }

            var collection = await _collectionService.FindCollectionById(collectable.CollectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var collectableItem = await _collectableService
                .FindCollectableById(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLowerInvariant().Contains(collection.Type.ToLowerInvariant()))
            {
                return BadRequest();
            }

            var retrievedCollectable = await _collectableService.FindCollectionCollectableById(collectionId, id);

            if (retrievedCollectable == null)
            {
                return NotFound();
            }

            retrievedCollectable.CollectionId = collectable.CollectionId;
            retrievedCollectable.CollectableId = collectable.CollectableId;

            _mapper.Map(collectable, retrievedCollectable);
            _collectableService.UpdateCollectionCollectable(retrievedCollectable);

            if (!await _collectableService.Save())
            {
                throw new Exception($"Updating collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCollectionCollectable")]
        public async Task<IActionResult> PartiallyUpdateCollectionCollectable(Guid collectionId, Guid id,
            [FromBody] JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var retrievedCollectable = await _collectableService.FindCollectionCollectableById(collectionId, id);

            if (retrievedCollectable == null)
            {
                return NotFound();
            }

            var patchedCollectable = _mapper.Map<CollectionCollectableUpdateDto>(retrievedCollectable);
            patchDoc.ApplyTo(patchedCollectable, ModelState);

            TryValidateModel(patchedCollectable);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (patchedCollectable.CollectionId == Guid.Empty)
            {
                patchedCollectable.CollectionId = collectionId;
            }

            var collection = await _collectionService.FindCollectionById(patchedCollectable.CollectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var collectableItem = await _collectableService
                .FindCollectableById(patchedCollectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLowerInvariant().Contains(collection.Type.ToLowerInvariant()))
            {
                return BadRequest();
            }

            retrievedCollectable.CollectionId = patchedCollectable.CollectionId;
            retrievedCollectable.CollectableId = patchedCollectable.CollectableId;

            _mapper.Map(patchedCollectable, retrievedCollectable);
            _collectableService.UpdateCollectionCollectable(retrievedCollectable);

            if (!await _collectableService.Save())
            {
                throw new Exception($"Patching collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectionCollectable")]
        public async Task<IActionResult> DeleteCollectionCollectable(Guid collectionId, Guid id)
        {
            var retrievedCollectable = await _collectableService.FindCollectionCollectableById(collectionId, id);

            if (retrievedCollectable == null)
            {
                return NotFound();
            }

            _collectableService.RemoveCollectionCollectable(retrievedCollectable);

            if (!await _collectableService.Save())
            {
                throw new Exception($"Deleting collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCollectionCollectablesOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateCollectionCollectablesResourceUri(CollectionCollectablesResourceParameters resourceParameters, 
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCollectionCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCollectionCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetCollectionCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateCollectionCollectableLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("GetCollectionCollectable",
                new { id }), "self", "GET"));

            links.Add(new LinkDto(Url.Link("CreateCollectionCollectable",
                new { }), "create_collectable", "POST"));

            links.Add(new LinkDto(Url.Link("UpdateCollectionCollectable",
                new { id }), "update_collectable", "PUT"));

            links.Add(new LinkDto(Url.Link("PartiallyUpdateCollectionCollectable",
                new { id }), "partially_update_collectable", "PATCH"));

            links.Add(new LinkDto(Url.Link("DeleteCollectionCollectable",
                new { id }), "delete_collectable", "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateCollectionCollectablesLinks
            (CollectionCollectablesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateCollectionCollectablesResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCollectionCollectablesResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCollectionCollectablesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}