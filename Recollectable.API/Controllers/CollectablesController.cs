using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Helpers;
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
    [Route("api/collections/{collectionId}/collectables")]
    public class CollectablesController : Controller
    {
        private ICollectableRepository _collectableRepository;
        private ICollectionRepository _collectionRepository;
        private IConditionRepository _conditionRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public CollectablesController(ICollectableRepository collectableRepository,
            ICollectionRepository collectionRepository, IConditionRepository conditionRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _collectableRepository = collectableRepository;
            _collectionRepository = collectionRepository;
            _conditionRepository = conditionRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectables")]
        public IActionResult GetCollectables(Guid collectionId, 
            CollectablesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CollectableDto, Collectable>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CollectableDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var collectablesFromRepo = _collectableRepository
                .GetCollectables(collectionId, resourceParameters);

            if (collectablesFromRepo == null)
            {
                return BadRequest();
            }

            var collectables = Mapper.Map<IEnumerable<CollectableDto>>(collectablesFromRepo);

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

                var linkedCollectionResource = new
                {
                    value = linkedCollectables,
                    links
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
        public IActionResult GetCollectable(Guid collectionId, Guid id, 
            [FromQuery] string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CollectableDto>(fields))
            {
                return BadRequest();
            }

            var collectableFromRepo = _collectableRepository.GetCollectable(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            var collectable = Mapper.Map<CollectableDto>(collectableFromRepo);

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
        public IActionResult CreateCollectable(Guid collectionId, 
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

            var collection = _collectionRepository.GetCollection(collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var collectableItem = _collectableRepository.GetCollectableItem(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLower().Contains(collection.Type.ToLower()))
            {
                return BadRequest();
            }

            var condition = _conditionRepository.GetCondition(collectable.ConditionId);

            if (condition == null)
            {
                return BadRequest();
            }

            var newCollectable = Mapper.Map<CollectionCollectable>(collectable);
            newCollectable.CollectionId = collectionId;
            newCollectable.Collection = collection;
            newCollectable.Collectable = collectableItem;
            newCollectable.Condition = condition;

            _collectableRepository.AddCollectable(newCollectable);

            if (!_collectableRepository.Save())
            {
                throw new Exception("Creating a collectable failed on save.");
            }

            var returnedCollectable = Mapper.Map<CollectableDto>(newCollectable);

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
        public IActionResult BlockCollectableCreation(Guid id)
        {
            if (_collectableRepository.CollectableExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCollectable")]
        public IActionResult UpdateCoin(Guid collectionId, Guid id,
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

            var collection = _collectionRepository.GetCollection(collectable.CollectionId);

            if (collection == null)
            {
                return BadRequest();
            }

            var collectableItem = _collectableRepository.GetCollectableItem(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLower().Contains(collection.Type.ToLower()))
            {
                return BadRequest();
            }

            if (!_conditionRepository.ConditionExists(collectable.ConditionId))
            {
                return BadRequest();
            }

            var collectableFromRepo = _collectableRepository.GetCollectable(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            collectableFromRepo.CollectionId = collectable.CollectionId;
            collectableFromRepo.CollectableId = collectable.CollectableId;
            collectableFromRepo.ConditionId = collectable.ConditionId;

            Mapper.Map(collectable, collectableFromRepo);
            _collectableRepository.UpdateCollectable(collectableFromRepo);

            if (!_collectableRepository.Save())
            {
                throw new Exception($"Updating collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCollectable")]
        public IActionResult PartiallyUpdateCollectable(Guid collectionId, Guid id,
            [FromBody] JsonPatchDocument<CollectableUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var collectableFromRepo = _collectableRepository.GetCollectable(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollectable = Mapper.Map<CollectableUpdateDto>(collectableFromRepo);
            patchDoc.ApplyTo(patchedCollectable, ModelState);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var collection = _collectionRepository.GetCollection(patchedCollectable.CollectionId);

            if (collection == null)
            {
                return BadRequest();
            }

            var collectableItem = _collectableRepository.GetCollectableItem(patchedCollectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLower().Contains(collection.Type.ToLower()))
            {
                return BadRequest();
            }

            if (!_conditionRepository.ConditionExists(patchedCollectable.ConditionId))
            {
                return BadRequest();
            }

            collectableFromRepo.CollectionId = patchedCollectable.CollectionId;
            collectableFromRepo.CollectableId = patchedCollectable.CollectableId;
            collectableFromRepo.ConditionId = patchedCollectable.ConditionId;

            Mapper.Map(patchedCollectable, collectableFromRepo);
            _collectableRepository.UpdateCollectable(collectableFromRepo);

            if (!_collectableRepository.Save())
            {
                throw new Exception($"Patching collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectable")]
        public IActionResult DeleteCollectable(Guid collectionId, Guid id)
        {
            var collectableFromRepo = _collectableRepository.GetCollectable(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            _collectableRepository.DeleteCollectable(collectableFromRepo);

            if (!_collectableRepository.Save())
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
                    return _urlHelper.Link("GetCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCollectables", new
                    {
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetCollectables", new
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
                links.Add(new LinkDto(_urlHelper.Link("GetCollectable",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("CreateCollectable",
                    new { }), "create_collectable", "POST"));

                links.Add(new LinkDto(_urlHelper.Link("UpdateCollectable",
                    new { id }), "update_collectable", "PUT"));

                links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateCollectable",
                    new { id }), "partially_update_collectable", "PATCH"));

                links.Add(new LinkDto(_urlHelper.Link("DeleteCollectable",
                    new { id }), "delete_collectable", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateCollectablesLinks
            (CollectablesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCollectablesResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET"));

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