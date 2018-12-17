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

        [HttpHead]
        [HttpGet(Name = "GetConditions")]
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

            if (mediaType == "application/json+hateoas")
            {
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
                var shapedConditions = conditions.ShapeData(resourceParameters.Fields);

                var linkedConditions = shapedConditions.Select(country =>
                {
                    var conditionAsDictionary = country as IDictionary<string, object>;
                    var countryLinks = CreateConditionLinks((Guid)conditionAsDictionary["Id"],
                        resourceParameters.Fields);

                    conditionAsDictionary.Add("links", countryLinks);

                    return conditionAsDictionary;
                });

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedConditions,
                    Links = links
                };

                return Ok(linkedCollectionResource);
            }
            else if (mediaType == "application/json")
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

                return Ok(conditions.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(conditions);
            }
        }

        [HttpGet("{id}", Name = "GetCondition")]
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

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateConditionLinks(id, fields);
                var linkedResource = condition.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(condition.ShapeData(fields));
            }
            else
            {
                return Ok(condition);
            }
        }

        [HttpPost(Name = "CreateCondition")]
        public async Task<IActionResult> CreateCondition([FromBody] ConditionCreationDto country,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (country == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var newCountry = _mapper.Map<Condition>(country);
            await _conditionService.CreateCondition(newCountry);

            if (!await _conditionService.Save())
            {
                throw new Exception("Creating a condition failed on save.");
            }

            var returnedCondition = _mapper.Map<ConditionDto>(newCountry);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateConditionLinks(returnedCondition.Id, null);
                var linkedResource = returnedCondition.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCountry",
                    new { id = returnedCondition.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCountry",
                    new { id = returnedCondition.Id },
                    returnedCondition);
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> BlockConditionCreation(Guid id)
        {
            if (await _conditionService.ConditionExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCondition")]
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

        [HttpPatch("{id}", Name = "PartiallyUpdateCondition")]
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

        [HttpDelete("{id}", Name = "DeleteCondition")]
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

            if (string.IsNullOrEmpty(fields))
            {
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
            }

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