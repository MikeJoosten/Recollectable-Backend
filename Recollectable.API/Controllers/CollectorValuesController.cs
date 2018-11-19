using AutoMapper;
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
    [Route("api/collector-values")]
    public class CollectorValuesController : Controller
    {
        private ICollectorValueRepository _collectorValueRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IMapper _mapper;

        public CollectorValuesController(ICollectorValueRepository collectorValueRepository, ITypeHelperService typeHelperService,
            IPropertyMappingService propertyMappingService, IMapper mapper)
        {
            _collectorValueRepository = collectorValueRepository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectorValues")]
        public async Task<IActionResult> GetCollectorValues(CollectorValuesResourceParameters resourceParameters,
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

            var collectorValuesFromRepo = await _collectorValueRepository.GetCollectorValues(resourceParameters);
            var collectorValues = _mapper.Map<IEnumerable<CollectorValueDto>>(collectorValuesFromRepo);

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

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedCollectorValues,
                    Links = links
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
        public async Task<IActionResult> GetCollectorValue(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CollectorValueDto>(fields))
            {
                return BadRequest();
            }

            var collectorValueFromRepo = await _collectorValueRepository.GetCollectorValueById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            var collectorValue = _mapper.Map<CollectorValueDto>(collectorValueFromRepo);

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
        public async Task<IActionResult> CreateCollectorValue([FromBody] CollectorValueCreationDto collectorValue,
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

            var newCollectorValue = _mapper.Map<CollectorValue>(collectorValue);
            _collectorValueRepository.AddCollectorValue(newCollectorValue);

            if (!await _collectorValueRepository.Save())
            {
                throw new Exception("Creating a collector value failed on save.");
            }

            var returnedCollectorValue = _mapper.Map<CollectorValueDto>(newCollectorValue);

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
        public async Task<IActionResult> BlockCollectorValueCreation(Guid id)
        {
            if (await _collectorValueRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCollectorValue")]
        public async Task<IActionResult> UpdateCollectorValue
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

            var collectorValueFromRepo = await _collectorValueRepository.GetCollectorValueById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(collectorValue, collectorValueFromRepo);
            _collectorValueRepository.UpdateCollectorValue(collectorValueFromRepo);

            if (!await _collectorValueRepository.Save())
            {
                throw new Exception($"Updating collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCollectorValue")]
        public async Task<IActionResult> PartiallyUpdateCollectorValue(Guid id,
            [FromBody] JsonPatchDocument<CollectorValueUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var collectorValueFromRepo = await _collectorValueRepository.GetCollectorValueById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            var patchedCollectorValue = _mapper.Map<CollectorValueUpdateDto>(collectorValueFromRepo);
            patchDoc.ApplyTo(patchedCollectorValue, ModelState);

            TryValidateModel(patchedCollectorValue);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(patchedCollectorValue, collectorValueFromRepo);
            _collectorValueRepository.UpdateCollectorValue(collectorValueFromRepo);

            if (!await _collectorValueRepository.Save())
            {
                throw new Exception($"Patching collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectorValue")]
        public async Task<IActionResult> DeleteCollectorValue(Guid id)
        {
            var collectorValueFromRepo = await _collectorValueRepository.GetCollectorValueById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            _collectorValueRepository.DeleteCollectorValue(collectorValueFromRepo);

            if (!await _collectorValueRepository.Save())
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
                    return Url.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCollectorValues", new
                    {
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetCollectorValues", new
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
                links.Add(new LinkDto(Url.Link("GetCollectorValue",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(Url.Link("CreateCollectorValue",
                    new { }), "create_collector_value", "POST"));

                links.Add(new LinkDto(Url.Link("UpdateCollectorValue",
                    new { id }), "update_collector_value", "PUT"));

                links.Add(new LinkDto(Url.Link("PartiallyUpdateCollectorValue",
                    new { id }), "partially_update_collector_value", "PATCH"));

                links.Add(new LinkDto(Url.Link("DeleteCollectorValue",
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