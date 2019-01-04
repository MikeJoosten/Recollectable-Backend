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
    [ApiVersion("1.0")]
    [Route("api/conditions")]
    public class ConditionsController : Controller
    {
        private IConditionService _conditionService;
        private IMapper _mapper;

        public ConditionsController(IConditionService conditionService, IMapper mapper)
        {
            _conditionService = conditionService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves conditions
        /// </summary>
        /// <returns>List of conditions</returns>
        /// <response code="200">Returns a list of conditions</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetConditions")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(ConditionDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetConditions(ConditionsResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<Condition>(resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<ConditionDto>(resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedConditions = await _conditionService.FindConditions(resourceParameters);
            var conditions = _mapper.Map<IEnumerable<ConditionDto>>(retrievedConditions);
            var shapedConditions = conditions.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(resourceParameters.Fields) &&
                    !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var paginationMetadata = new
                {
                    totalCount = retrievedConditions.TotalCount,
                    pageSize = retrievedConditions.PageSize,
                    currentPage = retrievedConditions.CurrentPage,
                    totalPages = retrievedConditions.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateConditionsLinks(resourceParameters,
                    retrievedConditions.HasNext, retrievedConditions.HasPrevious);

                var linkedConditions = shapedConditions.Select(condition =>
                {
                    var conditionAsDictionary = condition as IDictionary<string, object>;
                    var conditionLinks = CreateConditionLinks((Guid)conditionAsDictionary["Id"],
                        resourceParameters.Fields);

                    conditionAsDictionary.Add("links", conditionLinks);

                    return conditionAsDictionary;
                });

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedConditions,
                    Links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = retrievedConditions.HasPrevious ?
                    CreateConditionsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedConditions.HasNext ?
                    CreateConditionsResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedConditions.TotalCount,
                    pageSize = retrievedConditions.PageSize,
                    currentPage = retrievedConditions.CurrentPage,
                    totalPages = retrievedConditions.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(shapedConditions);
            }
        }

        /// <summary>
        /// Retrieves the requested condition by condition ID
        /// </summary>
        /// <param name="id">Condition ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested condition</returns>
        /// <response code="200">Returns the requested condition</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting condition ID</response>
        [HttpGet("{id}", Name = "GetCondition")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(ConditionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCondition(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!TypeHelper.TypeHasProperties<ConditionDto>(fields))
            {
                return BadRequest();
            }

            var retrievedCondition = await _conditionService.FindConditionById(id);

            if (retrievedCondition == null)
            {
                return NotFound();
            }

            var condition = _mapper.Map<ConditionDto>(retrievedCondition);
            var shapedCondition = condition.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateConditionLinks(id, fields);
                var linkedResource = shapedCondition as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedCondition);
            }
        }

        //TODO Add Sample request
        /// <summary>
        /// Creates a condition
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /conditions
        ///     {
        ///         
        ///     }
        /// </remarks>
        /// <param name="condition">Custom condition</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created condition</returns>
        /// <response code="201">Returns the newly created condition</response>
        /// <response code="400">Invalid condition</response>
        /// <response code="422">Invalid condition validation</response>
        [HttpPost(Name = "CreateCondition")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(ConditionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCondition([FromBody] ConditionCreationDto condition,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (condition == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var newCondition = _mapper.Map<Condition>(condition);
            await _conditionService.CreateCondition(newCondition);

            if (!await _conditionService.Save())
            {
                throw new Exception("Creating a condition failed on save.");
            }

            var returnedCondition = _mapper.Map<ConditionDto>(newCondition);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateConditionLinks(returnedCondition.Id, null);
                var linkedResource = returnedCondition.ShapeData(null) as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCondition",
                    new { id = returnedCondition.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCondition",
                    new { id = returnedCondition.Id },
                    returnedCondition);
            }
        }

        /// <summary>
        /// Invalid condition creation request
        /// </summary>
        /// <param name="id">Condition ID</param>
        /// <response code="404">Unexisting condition ID</response>
        /// <response code="409">Already existing condition ID</response>
        [HttpPost("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockConditionCreation(Guid id)
        {
            if (await _conditionService.ConditionExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        //TODO Add Sample request
        /// <summary>
        /// Updates a condition
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /conditions/{id}
        ///     {
        ///         
        ///     }
        /// </remarks>
        /// <param name="id">Condition ID</param>
        /// <param name="condition">Custom condition</param>
        /// <response code="204">Updated the condition successfully</response>
        /// <response code="400">Invalid condition</response>
        /// <response code="404">Unexisting condition ID</response>
        /// <response code="422">Invalid condition validation</response>
        [HttpPut("{id}", Name = "UpdateCondition")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCondition(Guid id, [FromBody] ConditionUpdateDto condition)
        {
            if (condition == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var retrievedCondition = await _conditionService.FindConditionById(id);

            if (retrievedCondition == null)
            {
                return NotFound();
            }

            _mapper.Map(condition, retrievedCondition);
            _conditionService.UpdateCondition(retrievedCondition);

            if (!await _conditionService.Save())
            {
                throw new Exception($"Updating condition {id} failed on save.");
            }

            return NoContent();
        }

        //TODO Add Sample request
        /// <summary>
        /// Update specific fields of a condition
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /conditions/{id}
        ///     [
        ///	        
        ///     ]
        /// </remarks>
        /// <param name="id">Condition ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        /// <response code="204">Updated the condition successfully</response>
        /// <response code="400">Invalid patch document</response>
        /// <response code="404">Unexisting condition ID</response>
        /// <response code="422">Invalid condition validation</response>
        [HttpPatch("{id}", Name = "PartiallyUpdateCondition")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PartiallyUpdateCondition(Guid id,
            [FromBody] JsonPatchDocument<ConditionUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var retrievedCondition = await _conditionService.FindConditionById(id);

            if (retrievedCondition == null)
            {
                return NotFound();
            }

            var patchedCondition = _mapper.Map<ConditionUpdateDto>(retrievedCondition);
            patchDoc.ApplyTo(patchedCondition, ModelState);

            TryValidateModel(patchedCondition);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(patchedCondition, retrievedCondition);
            _conditionService.UpdateCondition(retrievedCondition);

            if (!await _conditionService.Save())
            {
                throw new Exception($"Patching condition {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Removes a condition
        /// </summary>
        /// <param name="id">Condition ID</param>
        /// <response code="204">Removed the condition successfully</response>
        /// <response code="404">Unexisting condition ID</response>
        [HttpDelete("{id}", Name = "DeleteCondition")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCondition(Guid id)
        {
            var retrievedCondition = await _conditionService.FindConditionById(id);

            if (retrievedCondition == null)
            {
                return NotFound();
            }

            _conditionService.RemoveCondition(retrievedCondition);

            if (!await _conditionService.Save())
            {
                throw new Exception($"Deleting condition {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetConditionsOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateConditionsResourceUri(ConditionsResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        languageCode = resourceParameters.LanguageCode,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        languageCode = resourceParameters.LanguageCode,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        languageCode = resourceParameters.LanguageCode,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateConditionLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("GetCondition",
                new { id }), "self", "GET"));

            links.Add(new LinkDto(Url.Link("CreateCondition",
                new { }), "create_condition", "POST"));

            links.Add(new LinkDto(Url.Link("UpdateCondition",
                new { id }), "update_condition", "PUT"));

            links.Add(new LinkDto(Url.Link("PartiallyUpdateCondition",
                new { id }), "partially_update_condition", "PATCH"));

            links.Add(new LinkDto(Url.Link("DeleteCondition",
                new { id }), "delete_condition", "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateConditionsLinks
            (ConditionsResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateConditionsResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateConditionsResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateConditionsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}