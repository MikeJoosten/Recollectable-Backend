using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collections;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/collections")]
    public class CollectionsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IControllerService _controllerService;

        public CollectionsController(IUnitOfWork unitOfWork,
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollections")]
        public IActionResult GetCollections(CollectionsResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<CollectionDto, Collection>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<CollectionDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var collectionsFromRepo = _unitOfWork.CollectionRepository.Get(resourceParameters);
            var collections = Mapper.Map<IEnumerable<CollectionDto>>(collectionsFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = collectionsFromRepo.TotalCount,
                    pageSize = collectionsFromRepo.PageSize,
                    currentPage = collectionsFromRepo.CurrentPage,
                    totalPages = collectionsFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCollectionsLinks(resourceParameters,
                    collectionsFromRepo.HasNext, collectionsFromRepo.HasPrevious);
                var shapedCollections = collections.ShapeData(resourceParameters.Fields);

                var linkedCollections = shapedCollections.Select(collection =>
                {
                    var collectionAsDictionary = collection as IDictionary<string, object>;
                    var collectionLinks = CreateCollectionLinks((Guid)collectionAsDictionary["Id"],
                        resourceParameters.Fields);

                    collectionAsDictionary.Add("links", collectionLinks);

                    return collectionAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = linkedCollections,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else if (mediaType == "application/json")
            {
                var previousPageLink = collectionsFromRepo.HasPrevious ?
                    CreateCollectionsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = collectionsFromRepo.HasNext ?
                    CreateCollectionsResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = collectionsFromRepo.TotalCount,
                    pageSize = collectionsFromRepo.PageSize,
                    currentPage = collectionsFromRepo.CurrentPage,
                    totalPages = collectionsFromRepo.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(collections.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(collections);
            }
        }

        [HttpGet("{id}", Name = "GetCollection")]
        public IActionResult GetCollection(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.TypeHelperService.TypeHasProperties<CollectionDto>(fields))
            {
                return BadRequest();
            }

            var collectionFromRepo = _unitOfWork.CollectionRepository.GetById(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            var collection = Mapper.Map<CollectionDto>(collectionFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectionLinks(id, fields);
                var linkedResource = collection.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(collection.ShapeData(fields));
            }
            else
            {
                return Ok(collection);
            }
        }

        [HttpPost(Name = "CreateCollection")]
        public IActionResult CreateCollections([FromBody] CollectionCreationDto collection,
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

            var user = _unitOfWork.UserRepository.GetById(collection.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            var newCollection = Mapper.Map<Collection>(collection);
            newCollection.User = user;

            _unitOfWork.CollectionRepository.Add(newCollection);

            if (!_unitOfWork.Save())
            {
                throw new Exception("Creating a collection failed on save.");
            }

            var returnedCollection = Mapper.Map<CollectionDto>(newCollection);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectionLinks(returnedCollection.Id, null);
                var linkedResource = returnedCollection.ShapeData(null)
                    as IDictionary<string, object>;

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

        [HttpPost("{id}")]
        public IActionResult BlockCollectionCreation(Guid id)
        {
            if (_unitOfWork.CollectionRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCollection")]
        public IActionResult UpdateCollection(Guid id, [FromBody] CollectionUpdateDto collection)
        {
            if (collection == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_unitOfWork.UserRepository.Exists(collection.UserId))
            {
                return BadRequest();
            }

            var collectionFromRepo = _unitOfWork.CollectionRepository.GetById(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            collectionFromRepo.UserId = collection.UserId;

            Mapper.Map(collection, collectionFromRepo);
            _unitOfWork.CollectionRepository.Update(collectionFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Updating collection {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCollection")]
        public IActionResult PartiallyUpdateCollection(Guid id,
            [FromBody] JsonPatchDocument<CollectionUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var collectionFromRepo = _unitOfWork.CollectionRepository.GetById(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollection = Mapper.Map<CollectionUpdateDto>(collectionFromRepo);
            patchDoc.ApplyTo(patchedCollection, ModelState);

            TryValidateModel(patchedCollection);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_unitOfWork.UserRepository.Exists(patchedCollection.UserId))
            {
                return BadRequest();
            }

            collectionFromRepo.UserId = patchedCollection.UserId;

            Mapper.Map(patchedCollection, collectionFromRepo);
            _unitOfWork.CollectionRepository.Update(collectionFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching collection {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollection")]
        public IActionResult DeleteCollection(Guid id)
        {
            var collectionFromRepo = _unitOfWork.CollectionRepository.GetById(id);

            if (collectionFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.CollectionRepository.Delete(collectionFromRepo);

            if (!_unitOfWork.Save())
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

            if (string.IsNullOrEmpty(fields))
            {
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
            }

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