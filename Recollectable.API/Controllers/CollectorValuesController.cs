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

namespace Recollectable.API.Controllers
{
    [Route("api/collector-values")]
    public class CollectorValuesController : Controller
    {
        private ICollectorValueRepository _collectorValueRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;

        public CollectorValuesController(ICollectorValueRepository collectorValueRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService)
        {
            _collectorValueRepository = collectorValueRepository;
            _propertyMappingService = propertyMappingService;
        }

        [HttpGet(Name = "GetCollectorValues")]
        public IActionResult GetCollectorValues(CollectorValuesResourceParameters resourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CollectorValueDto, CollectorValue>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var collectorValuesFromRepo = _collectorValueRepository.GetCollectorValues(resourceParameters);

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
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var collectorValues = Mapper.Map<IEnumerable<CollectorValueDto>>(collectorValuesFromRepo);
            return Ok(collectorValues);
        }

        [HttpGet("{id}", Name = "GetCollectorValue")]
        public IActionResult GetCollectorValue(Guid id)
        {
            var collectorValueFromRepo = _collectorValueRepository.GetCollectorValue(id);

            if (collectorValueFromRepo == null)
            {
                return NotFound();
            }

            var collectorValue = Mapper.Map<CollectorValueDto>(collectorValueFromRepo);
            return Ok(collectorValue);
        }

        [HttpPost]
        public IActionResult CreateCollectorValue([FromBody] CollectorValueCreationDto collectorValue)
        {
            if (collectorValue == null)
            {
                return BadRequest();
            }

            var newCollectorValue = Mapper.Map<CollectorValue>(collectorValue);
            _collectorValueRepository.AddCollectorValue(newCollectorValue);

            if (!_collectorValueRepository.Save())
            {
                throw new Exception("Creating a collector value failed on save.");
            }

            var returnedCollectorValue = Mapper.Map<CollectorValueDto>(newCollectorValue);
            return CreatedAtRoute("GetCollectorValue", 
                new { id = returnedCollectorValue.Id }, 
                returnedCollectorValue);
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

        [HttpPut("{id}")]
        public IActionResult UpdateCollectorValue
            (Guid id, [FromBody] CollectorValueUpdateDto collectorValue)
        {
            if (collectorValue == null)
            {
                return BadRequest();
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

        [HttpPatch("{id}")]
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
            patchDoc.ApplyTo(patchedCollectorValue);

            Mapper.Map(patchedCollectorValue, collectorValueFromRepo);
            _collectorValueRepository.UpdateCollectorValue(collectorValueFromRepo);

            if (!_collectorValueRepository.Save())
            {
                throw new Exception($"Patching collector value {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
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

        private string CreateCollectorValuesResourceUri(CollectorValuesResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCollectorValues", new
                    {
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCollectorValues", new
                    {
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetCollectorValues", new
                    {
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }
    }
}