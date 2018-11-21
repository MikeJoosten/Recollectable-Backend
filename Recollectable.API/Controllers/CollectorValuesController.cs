using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;
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
    [Route("api/collector-values")]
    public class CollectorValuesController : Controller
    {
        private ICollectorValueService _collectorValueService;
        private IMapper _mapper;

        public CollectorValuesController(ICollectorValueService collectorValueService, IMapper mapper)
        {
            _collectorValueService = collectorValueService;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetCollectorValues")]
        public async Task<IActionResult> GetCollectorValues(CollectorValuesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<CollectorValue>(resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<CollectorValueDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedCollectorValues = await _collectorValueService.FindCollectorValues(resourceParameters);
            var collectorValues = _mapper.Map<IEnumerable<CollectorValueDto>>(retrievedCollectorValues);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = retrievedCollectorValues.TotalCount,
                    pageSize = retrievedCollectorValues.PageSize,
                    currentPage = retrievedCollectorValues.CurrentPage,
                    totalPages = retrievedCollectorValues.TotalPages,
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCollectorValuesLinks(resourceParameters,
                    retrievedCollectorValues.HasNext, retrievedCollectorValues.HasPrevious);
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
                var previousPageLink = retrievedCollectorValues.HasPrevious ?
                    CreateCollectorValuesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedCollectorValues.HasNext ?
                    CreateCollectorValuesResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedCollectorValues.TotalCount,
                    pageSize = retrievedCollectorValues.PageSize,
                    currentPage = retrievedCollectorValues.CurrentPage,
                    totalPages = retrievedCollectorValues.TotalPages,
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
            if (!TypeHelper.TypeHasProperties<CollectorValueDto>(fields))
            {
                return BadRequest();
            }

            var retrievedCollectorValue = await _collectorValueService.FindCollectorValueById(id);

            if (retrievedCollectorValue == null)
            {
                return NotFound();
            }

            var collectorValue = _mapper.Map<CollectorValueDto>(retrievedCollectorValue);

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
            await _collectorValueService.CreateCollectorValue(newCollectorValue);

            if (!await _collectorValueService.Save())
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
            if (await _collectorValueService.Exists(id))
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

            var retrievedCollectorValue = await _collectorValueService.FindCollectorValueById(id);

            if (retrievedCollectorValue == null)
            {
                return NotFound();
            }

            _mapper.Map(collectorValue, retrievedCollectorValue);
            _collectorValueService.UpdateCollectorValue(retrievedCollectorValue);

            if (!await _collectorValueService.Save())
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

            var retrievedCollectorValue = await _collectorValueService.FindCollectorValueById(id);

            if (retrievedCollectorValue == null)
            {
                return NotFound();
            }

            var patchedCollectorValue = _mapper.Map<CollectorValueUpdateDto>(retrievedCollectorValue);
            patchDoc.ApplyTo(patchedCollectorValue, ModelState);

            TryValidateModel(patchedCollectorValue);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(patchedCollectorValue, retrievedCollectorValue);
            _collectorValueService.UpdateCollectorValue(retrievedCollectorValue);

            if (!await _collectorValueService.Save())
            {
                throw new Exception($"Patching collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCollectorValue")]
        public async Task<IActionResult> DeleteCollectorValue(Guid id)
        {
            var collectorValueFromRepo = await _collectorValueService.FindCollectorValueById(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            _collectorValueService.RemoveCollectorValue(collectorValueFromRepo);

            if (!await _collectorValueService.Save())
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