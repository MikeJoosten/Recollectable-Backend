using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/collections/{collectionId}/collectables")]
    public class CollectablesController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IControllerService _controllerService;

        public CollectablesController(IUnitOfWork unitOfWork,
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectables")]
        public IActionResult GetCollectables(Guid collectionId, 
            CollectablesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<CollectableDto, Collectable>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<CollectableDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var collectablesFromRepo = _unitOfWork.CollectableRepository
                .Get(collectionId, resourceParameters);

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
            if (!_controllerService.TypeHelperService.TypeHasProperties<CollectableDto>(fields))
            {
                return BadRequest();
            }

            var collectableFromRepo = _unitOfWork.CollectableRepository.GetById(collectionId, id);

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

            var collection = _unitOfWork.CollectionRepository.GetById(collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var collectableItem = _unitOfWork.CollectableRepository
                .GetCollectableItem(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLower().Contains(collection.Type.ToLower()))
            {
                return BadRequest();
            }

            var newCollectable = Mapper.Map<CollectionCollectable>(collectable);
            newCollectable.CollectionId = collectionId;
            newCollectable.Collection = collection;
            newCollectable.Collectable = collectableItem;

            _unitOfWork.CollectableRepository.Add(newCollectable);

            if (!_unitOfWork.Save())
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
        public IActionResult BlockCollectableCreation(Guid collectionId, Guid id)
        {
            if (_unitOfWork.CollectableRepository.Exists(collectionId, id))
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

            var collection = _unitOfWork.CollectionRepository.GetById(collectable.CollectionId);

            if (collection == null)
            {
                return BadRequest();
            }

            var collectableItem = _unitOfWork.CollectableRepository
                .GetCollectableItem(collectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLower().Contains(collection.Type.ToLower()))
            {
                return BadRequest();
            }

            var collectableFromRepo = _unitOfWork.CollectableRepository.GetById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            collectableFromRepo.CollectionId = collectable.CollectionId;
            collectableFromRepo.CollectableId = collectable.CollectableId;

            Mapper.Map(collectable, collectableFromRepo);
            _unitOfWork.CollectableRepository.Update(collectableFromRepo);

            if (!_unitOfWork.Save())
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

            var collectableFromRepo = _unitOfWork.CollectableRepository.GetById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollectable = Mapper.Map<CollectableUpdateDto>(collectableFromRepo);
            patchDoc.ApplyTo(patchedCollectable, ModelState);

            TryValidateModel(patchedCollectable);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var collection = _unitOfWork.CollectionRepository.GetById(patchedCollectable.CollectionId);

            if (collection == null)
            {
                return BadRequest();
            }

            var collectableItem = _unitOfWork.CollectableRepository
                .GetCollectableItem(patchedCollectable.CollectableId);

            if (collectableItem == null || !collectableItem.GetType().ToString()
                .ToLower().Contains(collection.Type.ToLower()))
            {
                return BadRequest();
            }

            collectableFromRepo.CollectionId = patchedCollectable.CollectionId;
            collectableFromRepo.CollectableId = patchedCollectable.CollectableId;

            Mapper.Map(patchedCollectable, collectableFromRepo);
            _unitOfWork.CollectableRepository.Update(collectableFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching collectable {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectable")]
        public IActionResult DeleteCollectable(Guid collectionId, Guid id)
        {
            var collectableFromRepo = _unitOfWork.CollectableRepository.GetById(collectionId, id);

            if (collectableFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.CollectableRepository.Delete(collectableFromRepo);

            if (!_unitOfWork.Save())
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