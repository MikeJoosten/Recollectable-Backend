﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.Core.DTOs.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using Recollectable.Core.Shared.DTOs;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/conditions")]
    public class ConditionsController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IControllerService _controllerService;

        public ConditionsController(IUnitOfWork unitOfWork,
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetConditions")]
        public IActionResult GetConditions(ConditionsResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<ConditionDto, Condition>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<ConditionDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var conditionsFromRepo = _unitOfWork.ConditionRepository.Get(resourceParameters);
            var conditions = Mapper.Map<IEnumerable<ConditionDto>>(conditionsFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = conditionsFromRepo.TotalCount,
                    pageSize = conditionsFromRepo.PageSize,
                    currentPage = conditionsFromRepo.CurrentPage,
                    totalPages = conditionsFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateConditionsLinks(resourceParameters,
                    conditionsFromRepo.HasNext, conditionsFromRepo.HasPrevious);
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
            else if (mediaType == "application/json")
            {
                var previousPageLink = conditionsFromRepo.HasPrevious ?
                    CreateConditionsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = conditionsFromRepo.HasNext ?
                    CreateConditionsResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = conditionsFromRepo.TotalCount,
                    pageSize = conditionsFromRepo.PageSize,
                    currentPage = conditionsFromRepo.CurrentPage,
                    totalPages = conditionsFromRepo.TotalPages,
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
        public IActionResult GetCondition(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.TypeHelperService.TypeHasProperties<ConditionDto>(fields))
            {
                return BadRequest();
            }

            var conditionFromRepo = _unitOfWork.ConditionRepository.GetById(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            var condition = Mapper.Map<ConditionDto>(conditionFromRepo);

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
        public IActionResult CreateCondition([FromBody] ConditionCreationDto condition,
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

            var newCondition = Mapper.Map<Condition>(condition);
            _unitOfWork.ConditionRepository.Add(newCondition);

            if (!_unitOfWork.Save())
            {
                throw new Exception("Creating a condition failed on save.");
            }

            var returnedCondition = Mapper.Map<ConditionDto>(newCondition);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateConditionLinks(returnedCondition.Id, null);
                var linkedResource = returnedCondition.ShapeData(null)
                    as IDictionary<string, object>;

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

        [HttpPost("{id}")]
        public IActionResult BlockConditionCreation(Guid id)
        {
            if (_unitOfWork.ConditionRepository.Exists(id))
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

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var conditionFromRepo = _unitOfWork.ConditionRepository.GetById(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(condition, conditionFromRepo);
            _unitOfWork.ConditionRepository.Update(conditionFromRepo);

            if (!_unitOfWork.Save())
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

            var conditionFromRepo = _unitOfWork.ConditionRepository.GetById(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            var patchedCondition = Mapper.Map<ConditionUpdateDto>(conditionFromRepo);
            patchDoc.ApplyTo(patchedCondition, ModelState);

            TryValidateModel(patchedCondition);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(patchedCondition, conditionFromRepo);
            _unitOfWork.ConditionRepository.Update(conditionFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching condition {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCondition")]
        public IActionResult DeleteCondition(Guid id)
        {
            var conditionFromRepo = _unitOfWork.ConditionRepository.GetById(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.ConditionRepository.Delete(conditionFromRepo);

            if (!_unitOfWork.Save())
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
                    return _controllerService.UrlHelper.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _controllerService.UrlHelper.Link("GetConditions", new
                    {
                        grade = resourceParameters.Grade,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _controllerService.UrlHelper.Link("GetConditions", new
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
                links.Add(new LinkDto(_controllerService.UrlHelper.Link("GetCondition",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("CreateCondition",
                    new { }), "create_condition", "POST"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("UpdateCondition",
                    new { id }), "update_condition", "PUT"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("PartiallyUpdateCondition",
                    new { id }), "partially_update_condition", "PATCH"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("DeleteCondition",
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