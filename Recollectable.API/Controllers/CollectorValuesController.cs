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
    [Route("api/collector-values")]
    public class CollectorValuesController : Controller
    {
        private ICollectorValueRepository _collectorValueRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public CollectorValuesController(ICollectorValueRepository collectorValueRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _collectorValueRepository = collectorValueRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectorValues")]
        public IActionResult GetCollectorValues(CollectorValuesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CollectorValueDto, CollectorValue>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CollectorValueDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var collectorValuesFromRepo = _collectorValueRepository.GetCollectorValues(resourceParameters);
            var collectorValues = Mapper.Map<IEnumerable<CollectorValueDto>>(collectorValuesFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = collectorValuesFromRepo.TotalCount,
                    pageSize = collectorValuesFromRepo.PageSize,
                    currentPage = collectorValuesFromRepo.CurrentPage,
                    totalPages = collectorValuesFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCollectorValuesLinks(resourceParameters,
                    collectorValuesFromRepo.HasNext, collectorValuesFromRepo.HasPrevious);
                var shapedCollectorValues = collectorValues.ShapeData(resourceParameters.Fields);

                var linkedCollectorValues = shapedCollectorValues.Select(collectorValue =>
                {
                    var collectorValueAsDictionary = collectorValue as IDictionary<string, object>;
                    var collectorValueLinks = CreateCollectorValueLinks
                        ((Guid)collectorValueAsDictionary["Id"], resourceParameters.Fields);

                    collectorValueAsDictionary.Add("links", collectorValueLinks);

                    return collectorValueAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = linkedCollectorValues,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else if (mediaType == "application/json")
            {
                var previousPageLink = collectorValuesFromRepo.HasPrevious ?
                    CreateCollectorValuesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = collectorValuesFromRepo.HasNext ?
                    CreateCollectorValuesResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = collectorValuesFromRepo.TotalCount,
                    pageSize = collectorValuesFromRepo.PageSize,
                    currentPage = collectorValuesFromRepo.CurrentPage,
                    totalPages = collectorValuesFromRepo.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(collectorValues.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(collectorValues);
            }
        }

        [HttpGet("{id}", Name = "GetCollectorValue")]
        public IActionResult GetCollectorValue(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CollectorValueDto>(fields))
            {
                return BadRequest();
            }

            var collectorValueFromRepo = _collectorValueRepository.GetCollectorValue(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            var collectorValue = Mapper.Map<CollectorValueDto>(collectorValueFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectorValueLinks(id, fields);
                var linkedResource = collectorValue.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(collectorValue.ShapeData(fields));
            }
            else
            {
                return Ok(collectorValue);
            }
        }

        [HttpPost(Name = "CreateCollectorValue")]
        public IActionResult CreateCollectorValue([FromBody] CollectorValueCreationDto collectorValue,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (collectorValue == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var newCollectorValue = Mapper.Map<CollectorValue>(collectorValue);
            _collectorValueRepository.AddCollectorValue(newCollectorValue);

            if (!_collectorValueRepository.Save())
            {
                throw new Exception("Creating a collector value failed on save.");
            }

            var returnedCollectorValue = Mapper.Map<CollectorValueDto>(newCollectorValue);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCollectorValueLinks(returnedCollectorValue.Id, null);
                var linkedResource = returnedCollectorValue.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCollectorValue",
                    new { id = returnedCollectorValue.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCollectorValue",
                    new { id = returnedCollectorValue.Id },
                    returnedCollectorValue);
            }
        }

        [HttpPost("{id}")]
        public IActionResult BlockCollectorValueCreation(Guid id)
        {
            if (_collectorValueRepository.CollectorValueExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCollectorValue")]
        public IActionResult UpdateCollectorValue
            (Guid id, [FromBody] CollectorValueUpdateDto collectorValue)
        {
            if (collectorValue == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var collectorValueFromRepo = _collectorValueRepository.GetCollectorValue(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(collectorValue, collectorValueFromRepo);
            _collectorValueRepository.UpdateCollectorValue(collectorValueFromRepo);

            if (!_collectorValueRepository.Save())
            {
                throw new Exception($"Updating collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCollectorValue")]
        public IActionResult PartiallyUpdateCollectorValue(Guid id,
            [FromBody] JsonPatchDocument<CollectorValueUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var collectorValueFromRepo = _collectorValueRepository.GetCollectorValue(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollectorValue = Mapper.Map<CollectorValueUpdateDto>(collectorValueFromRepo);
            patchDoc.ApplyTo(patchedCollectorValue, ModelState);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(patchedCollectorValue, collectorValueFromRepo);
            _collectorValueRepository.UpdateCollectorValue(collectorValueFromRepo);

            if (!_collectorValueRepository.Save())
            {
                throw new Exception($"Patching collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectorValue")]
        public IActionResult DeleteCollectorValue(Guid id)
        {
            var collectorValueFromRepo = _collectorValueRepository.GetCollectorValue(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            _collectorValueRepository.DeleteCollectorValue(collectorValueFromRepo);

            if (!_collectorValueRepository.Save())
            {
                throw new Exception($"Deleting collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCollectorValuesOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateCollectorValuesResourceUri(CollectorValuesResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateCollectorValueLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(_urlHelper.Link("GetCollectorValue",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("CreateCollectorValue",
                    new { }), "create_collector_value", "POST"));

                links.Add(new LinkDto(_urlHelper.Link("UpdateCollectorValue",
                    new { id }), "update_collector_value", "PUT"));

                links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateCollectorValue",
                    new { id }), "partially_update_collector_value", "PATCH"));

                links.Add(new LinkDto(_urlHelper.Link("DeleteCollectorValue",
                    new { id }), "delete_collector_value", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateCollectorValuesLinks
            (CollectorValuesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCollectorValuesResourceUri
                (resourceParameters, ResourceUriType.Current), 
                "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCollectorValuesResourceUri
                    (resourceParameters, ResourceUriType.NextPage), 
                    "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCollectorValuesResourceUri
                    (resourceParameters, ResourceUriType.PreviousPage), 
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}