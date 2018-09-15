using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.DTOs.Common;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Enums;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/collector-values")]
    public class CollectorValuesController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IControllerService _controllerService;

        public CollectorValuesController(IUnitOfWork unitOfWork,
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectorValues")]
        public IActionResult GetCollectorValues(CollectorValuesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<CollectorValueDto, CollectorValue>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<CollectorValueDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var collectorValuesFromRepo = _unitOfWork.CollectorValueRepository.Get(resourceParameters);
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
            if (!_controllerService.TypeHelperService.TypeHasProperties<CollectorValueDto>(fields))
            {
                return BadRequest();
            }

            var collectorValueFromRepo = _unitOfWork.CollectorValueRepository.GetById(id);

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
            _unitOfWork.CollectorValueRepository.Add(newCollectorValue);

            if (!_unitOfWork.Save())
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
            if (_unitOfWork.CollectorValueRepository.Exists(id))
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

            var collectorValueFromRepo = _unitOfWork.CollectorValueRepository.GetById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(collectorValue, collectorValueFromRepo);
            _unitOfWork.CollectorValueRepository.Update(collectorValueFromRepo);

            if (!_unitOfWork.Save())
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

            var collectorValueFromRepo = _unitOfWork.CollectorValueRepository.GetById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollectorValue = Mapper.Map<CollectorValueUpdateDto>(collectorValueFromRepo);
            patchDoc.ApplyTo(patchedCollectorValue, ModelState);

            TryValidateModel(patchedCollectorValue);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(patchedCollectorValue, collectorValueFromRepo);
            _unitOfWork.CollectorValueRepository.Update(collectorValueFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectorValue")]
        public IActionResult DeleteCollectorValue(Guid id)
        {
            var collectorValueFromRepo = _unitOfWork.CollectorValueRepository.GetById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.CollectorValueRepository.Delete(collectorValueFromRepo);

            if (!_unitOfWork.Save())
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
                    return _controllerService.UrlHelper.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _controllerService.UrlHelper.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _controllerService.UrlHelper.Link("GetCollectorValues", new
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
                links.Add(new LinkDto(_controllerService.UrlHelper.Link("GetCollectorValue",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("CreateCollectorValue",
                    new { }), "create_collector_value", "POST"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("UpdateCollectorValue",
                    new { id }), "update_collector_value", "PUT"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("PartiallyUpdateCollectorValue",
                    new { id }), "partially_update_collector_value", "PATCH"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("DeleteCollectorValue",
                    new { id }), "delete_collector_value", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateCollectorValuesLinks
            (CollectorValuesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateCollectorValuesResourceUri
                (resourceParameters, ResourceUriType.Current),
                "self", "GET")
            };

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