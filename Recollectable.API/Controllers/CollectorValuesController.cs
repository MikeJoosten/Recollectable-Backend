using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
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

        /// <summary>
        /// Retrieves collector values
        /// </summary>
        /// <returns>List of collector values</returns>
        /// <response code="200">Returns a list of collector values</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetCollectorValues")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CollectorValueDto), 200)]
        [ProducesResponseType(400)]
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
            var shapedCollectorValues = collectorValues.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(resourceParameters.Fields) &&
                    !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

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
            else
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

                return Ok(shapedCollectorValues);
            }
        }

        /// <summary>
        /// Retrieves the requested collector value by collector value ID
        /// </summary>
        /// <param name="id">Collector value ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested collector value</returns>
        /// <response code="200">Returns the requested collector value</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting collector value ID</response>
        [HttpGet("{id}", Name = "GetCollectorValue")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CoinDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
            var shapedCollectorValue = collectorValue.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateCollectorValueLinks(id, fields);
                var linkedResource = shapedCollectorValue as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedCollectorValue);
            }
        }

        /// <summary>
        /// Creates a collector value
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /collector-values
        ///     {
        ///         "g4": 440,
        ///         "vG8": 600,
        ///         "f12": 850,
        ///         "vF20": 945,
        ///         "xF40": 1450,
        ///         "aU50": 1650,
        ///         "mS60": 3250,
        ///         "mS63": 5750,
        ///         "mS65": 90000,
        ///         "pF65": 15700
        ///     }
        /// </remarks>
        /// <param name="collectorValue">Custom collector value</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created collector value</returns>
        /// <response code="201">Returns the newly created collector value</response>
        /// <response code="400">Invalid collector value</response>
        /// <response code="422">Invalid collector value validation</response>
        [HttpPost(Name = "CreateCollectorValue")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CollectorValueDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
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
                var linkedResource = returnedCollectorValue.ShapeData(null) as IDictionary<string, object>;

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

        /// <summary>
        /// Invalid collector value creation request
        /// </summary>
        /// <param name="id">Collector value ID</param>
        /// <response code="404">Unexisting collector value ID</response>
        /// <response code="409">Already existing collector value ID</response>
        [HttpPost("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockCollectorValueCreation(Guid id)
        {
            if (await _collectorValueService.CollectorValueExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        /// <summary>
        /// Updates a collector value
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /collector-values/{id}
        ///     {
        ///         "g4": 7.9,
        ///         "vG8": 8.5,
        ///         "f12": 9.6,
        ///         "vF20": 12,
        ///         "xF40": 28,
        ///         "aU50": 72.5,
        ///         "mS60": 125,
        ///         "mS65": 465
        ///     }
        /// </remarks>
        /// <param name="id">Collector value ID</param>
        /// <param name="collectorValue">Custom collector value</param>
        /// <response code="204">Updated the collector value successfully</response>
        /// <response code="400">Invalid collector value</response>
        /// <response code="404">Unexisting collector value ID</response>
        /// <response code="422">Invalid collector value validation</response>
        [HttpPut("{id}", Name = "UpdateCollectorValue")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
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

        /// <summary>
        /// Update specific fields of a collector value
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /collector-values/{id}
        ///     [
        ///	        { "op": "replace", "path": "/mS65", "value": 2850 },
        ///	        { "op": "copy", "from": "/g4", "path": "/f12" },
        ///	        { "op": "move", "from": "/xF40", "path": "/aU50" },
        ///	        { "op": "remove", "path": "/pF65" }	        
        ///     ]
        /// </remarks>
        /// <param name="id">Collector value ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        /// <response code="204">Updated the collector value successfully</response>
        /// <response code="400">Invalid patch document</response>
        /// <response code="404">Unexisting collector value ID</response>
        /// <response code="422">Invalid collector value validation</response>
        [HttpPatch("{id}", Name = "PartiallyUpdateCollectorValue")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
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

        /// <summary>
        /// Removes a collector value
        /// </summary>
        /// <param name="id">Collector value ID</param>
        /// <response code="204">Removed the collector value successfully</response>
        /// <response code="404">Unexisting collector value ID</response>
        [HttpDelete("{id}", Name = "DeleteCollectorValue")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
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