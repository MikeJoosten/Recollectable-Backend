using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    [Route("api/conditions")]
    public class ConditionsController : Controller
    {
        private IConditionRepository _conditionRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public ConditionsController(IConditionRepository conditionRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _conditionRepository = conditionRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetConditions")]
        public IActionResult GetConditions(ConditionsResourceParameters resourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ConditionDto, Condition>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<ConditionDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var conditionsFromRepo = _conditionRepository.GetConditions(resourceParameters);

            var paginationMetadata = new
            {
                totalCount = conditionsFromRepo.TotalCount,
                pageSize = conditionsFromRepo.PageSize,
                currentPage = conditionsFromRepo.CurrentPage,
                totalPages = conditionsFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var conditions = Mapper.Map<IEnumerable<ConditionDto>>(conditionsFromRepo);
            var links = CreateConditionsLinks(resourceParameters, conditionsFromRepo.HasNext,
                conditionsFromRepo.HasPrevious);
            var shapedConditions = conditions.ShapeData(resourceParameters.Fields);

            var linkedConditions = shapedConditions.Select(condition =>
            {
                var conditionAsDictionary = condition as IDictionary<string, object>;
                var conditionLinks = CreateConditionLinks((Guid)conditionAsDictionary["Id"],
                    resourceParameters.Fields);

                conditionAsDictionary.Add("links", conditionLinks);

                return conditionAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = linkedConditions,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpGet("{id}", Name = "GetCondition")]
        public IActionResult GetCondition(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<ConditionDto>(fields))
            {
                return BadRequest();
            }

            var conditionFromRepo = _conditionRepository.GetCondition(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            var condition = Mapper.Map<ConditionDto>(conditionFromRepo);
            var links = CreateConditionLinks(id, fields);
            var linkedResource = condition.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return Ok(linkedResource);
        }

        [HttpPost(Name = "CreateCondition")]
        public IActionResult CreateCondition([FromBody] ConditionCreationDto condition)
        {
            if (condition == null)
            {
                return BadRequest();
            }

            var newCondition = Mapper.Map<Condition>(condition);
            _conditionRepository.AddCondition(newCondition);

            if (!_conditionRepository.Save())
            {
                throw new Exception("Creating a condition failed on save.");
            }

            var returnedCondition = Mapper.Map<ConditionDto>(newCondition);
            var links = CreateConditionLinks(returnedCondition.Id, null);
            var linkedResource = returnedCondition.ShapeData(null)
                as IDictionary<string, object>;

            linkedResource.Add("links", links);

            return CreatedAtRoute("GetCondition",
                new { id = returnedCondition.Id },
                linkedResource);
        }

        [HttpPost("{id}")]
        public IActionResult BlockConditionCreation(Guid id)
        {
            if (_conditionRepository.ConditionExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCondition")]
        public IActionResult UpdateCondition(Guid id, [FromBody] ConditionUpdateDto condition)
        {
            if (condition == null)
            {
                return BadRequest();
            }

            var conditionFromRepo = _conditionRepository.GetCondition(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(condition, conditionFromRepo);
            _conditionRepository.UpdateCondition(conditionFromRepo);

            if (!_conditionRepository.Save())
            {
                throw new Exception($"Updating condition {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCondition")]
        public IActionResult PartiallyUpdateCondition(Guid id,
            [FromBody] JsonPatchDocument<ConditionUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var conditionFromRepo = _conditionRepository.GetCondition(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            var patchedCondition = Mapper.Map<ConditionUpdateDto>(conditionFromRepo);
            patchDoc.ApplyTo(patchedCondition);

            Mapper.Map(patchedCondition, conditionFromRepo);
            _conditionRepository.UpdateCondition(conditionFromRepo);

            if (!_conditionRepository.Save())
            {
                throw new Exception($"Patching condition {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCondition")]
        public IActionResult DeleteCondition(Guid id)
        {
            var conditionFromRepo = _conditionRepository.GetCondition(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            _conditionRepository.DeleteCondition(conditionFromRepo);

            if (!_conditionRepository.Save())
            {
                throw new Exception($"Deleting condition {id} failed on save.");
            }

            return NoContent();
        }

        private string CreateConditionsResourceUri(ConditionsResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
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
                links.Add(new LinkDto(_urlHelper.Link("GetCondition",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("CreateCondition",
                    new { }), "create_condition", "POST"));

                links.Add(new LinkDto(_urlHelper.Link("UpdateCondition",
                    new { id }), "update_condition", "PUT"));

                links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateCondition",
                    new { id }), "partially_update_condition", "PATCH"));

                links.Add(new LinkDto(_urlHelper.Link("DeleteCondition",
                    new { id }), "delete_condition", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateConditionsLinks
            (ConditionsResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateConditionsResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET"));

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