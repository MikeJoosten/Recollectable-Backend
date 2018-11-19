﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    //TODO Add Authorization
    [ApiVersion("1.0")]
    [Route("api/collections/{collectionId}/collectables")]
    public class CollectablesController : Controller
    {
        private ICollectableRepository _collectableRepository;
        private ICollectionRepository _collectionRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IMapper _mapper;

        public CollectablesController(ICollectableRepository collectableRepository, ICollectionRepository collectionRepository,
            ITypeHelperService typeHelperService, IPropertyMappingService propertyMappingService, IMapper mapper)
        {
            _collectableRepository = collectableRepository;
            _collectionRepository = collectionRepository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectables")]
        public async Task<IActionResult> GetCollectables(Guid collectionId, 
            CollectablesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CollectableDto, CollectionCollectable>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CollectableDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var collectablesFromRepo = await _collectableRepository
                .GetCollectables(collectionId, resourceParameters);

            if (collectablesFromRepo == null)
            {
                return NotFound();
            }

            var collectables = _mapper.Map<IEnumerable<CollectableDto>>(collectablesFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = collectablesFromRepo.TotalCount,
                    pageSize = collectablesFromRepo.PageSize,
                    currentPage = collectablesFromRepo.CurrentPage,
                    totalPages = collectablesFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCollectablesLinks(resourceParameters,
                    collectablesFromRepo.HasNext, collectablesFromRepo.HasPrevious);
                var shapedCollectables = collectables.ShapeData(resourceParameters.Fields);

                var linkedCollectables = shapedCollectables.Select(collectable =>
                {
                    var collectableAsDictionary = collectable as IDictionary<string, object>;
                    var collectableLinks = CreateCollectableLinks((Guid)collectableAsDictionary["Id"],
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
            else if (mediaType == "application/json")
            {
                var previousPageLink = collectablesFromRepo.HasPrevious ?
                    CreateCollectablesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = collectablesFromRepo.HasNext ?
                    CreateCollectablesResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = collectablesFromRepo.TotalCount,
                    pageSize = collectablesFromRepo.PageSize,
                    currentPage = collectablesFromRepo.CurrentPage,
                    totalPages = collectablesFromRepo.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(collectables.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(collectables);
            }
        }

        [HttpGet("{id}", Name = "GetCollectable")]
        public async Task<IActionResult> GetCollectable(Guid collectionId, Guid id, 
            [FromQuery] string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CollectableDto>(fields))
            {
                return BadRequest();
            }

            var collectableFromRepo = await _collectableRepository.GetCollectableById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            var collectable = _mapper.Map<CollectableDto>(collectableFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectableLinks(id, fields);
                var linkedResource = collectable.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(collectable.ShapeData(fields));
            }
            else
            {
                return Ok(collectable);
            }
        }

        [HttpPost(Name = "CreateCollectable")]
        public async Task<IActionResult> CreateCollectable(Guid collectionId, 
            [FromBody] CollectableCreationDto collectable,
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

            var collection = await _collectionRepository.GetCollectionById(collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var collectableItem = await _collectableRepository
                .GetCollectableItem(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLowerInvariant().Contains(collection.Type.ToLowerInvariant()))
            {
                return BadRequest();
            }

            var newCollectable = _mapper.Map<CollectionCollectable>(collectable);
            newCollectable.CollectionId = collectionId;
            newCollectable.Collection = collection;
            newCollectable.Collectable = collectableItem;

            _collectableRepository.AddCollectable(newCollectable);

            if (!await _collectableRepository.Save())
            {
                throw new Exception("Creating a collectable failed on save.");
            }

            var returnedCollectable = _mapper.Map<CollectableDto>(newCollectable);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectableLinks(returnedCollectable.Id, null);
                var linkedResource = returnedCollectable.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCollectable",
                    new { id = returnedCollectable.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCollectable",
                    new { id = returnedCollectable.Id },
                    returnedCollectable);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> BlockCollectableCreation(Guid collectionId, Guid id)
        {
            if (await _collectableRepository.Exists(collectionId, id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCollectable")]
        public async Task<IActionResult> UpdateCollectable(Guid collectionId, Guid id,
            [FromBody] CollectableUpdateDto collectable)
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

            var collection = await _collectionRepository.GetCollectionById(collectable.CollectionId);

            if (collection == null)
            {
                return BadRequest();
            }

            var collectableItem = await _collectableRepository
                .GetCollectableItem(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLowerInvariant().Contains(collection.Type.ToLowerInvariant()))
            {
                return BadRequest();
            }

            var collectableFromRepo = await _collectableRepository.GetCollectableById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            collectableFromRepo.CollectionId = collectable.CollectionId;
            collectableFromRepo.CollectableId = collectable.CollectableId;

            _mapper.Map(collectable, collectableFromRepo);
            _collectableRepository.UpdateCollectable(collectableFromRepo);

            if (!await _collectableRepository.Save())
            {
                throw new Exception($"Updating collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCollectable")]
        public async Task<IActionResult> PartiallyUpdateCollectable(Guid collectionId, Guid id,
            [FromBody] JsonPatchDocument<CollectableUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var collectableFromRepo = await _collectableRepository.GetCollectableById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollectable = _mapper.Map<CollectableUpdateDto>(collectableFromRepo);
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

            var collection = await _collectionRepository.GetCollectionById(patchedCollectable.CollectionId);

            if (collection == null)
            {
                return BadRequest();
            }

            var collectableItem = await _collectableRepository
                .GetCollectableItem(patchedCollectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLowerInvariant().Contains(collection.Type.ToLowerInvariant()))
            {
                return BadRequest();
            }

            collectableFromRepo.CollectionId = patchedCollectable.CollectionId;
            collectableFromRepo.CollectableId = patchedCollectable.CollectableId;

            _mapper.Map(patchedCollectable, collectableFromRepo);
            _collectableRepository.UpdateCollectable(collectableFromRepo);

            if (!await _collectableRepository.Save())
            {
                throw new Exception($"Patching collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectable")]
        public async Task<IActionResult> DeleteCollectable(Guid collectionId, Guid id)
        {
            var collectableFromRepo = await _collectableRepository.GetCollectableById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            _collectableRepository.DeleteCollectable(collectableFromRepo);

            if (!await _collectableRepository.Save())
            {
                throw new Exception($"Deleting collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCollectablesOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateCollectablesResourceUri(CollectablesResourceParameters resourceParameters, 
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetCollectables", new
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

        private IEnumerable<LinkDto> CreateCollectableLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(Url.Link("GetCollectable",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(Url.Link("CreateCollectable",
                    new { }), "create_collectable", "POST"));

                links.Add(new LinkDto(Url.Link("UpdateCollectable",
                    new { id }), "update_collectable", "PUT"));

                links.Add(new LinkDto(Url.Link("PartiallyUpdateCollectable",
                    new { id }), "partially_update_collectable", "PATCH"));

                links.Add(new LinkDto(Url.Link("DeleteCollectable",
                    new { id }), "delete_collectable", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateCollectablesLinks
            (CollectablesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateCollectablesResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCollectablesResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCollectablesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}