using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Models;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [Route("api/collector-values")]
    public class CollectorValuesController : Controller
    {
        private ICollectorValueRepository _collectorValueRepository;

        public CollectorValuesController(ICollectorValueRepository collectorValueRepository)
        {
            _collectorValueRepository = collectorValueRepository;
        }

        [HttpGet]
        public IActionResult GetCollectorValues()
        {
            var collectorValuesFromRepo = _collectorValueRepository.GetCollectorValues();
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
                var newCollectorValue = Mapper.Map<CollectorValue>(collectorValue);
                newCollectorValue.Id = id;

                _collectorValueRepository.AddCollectorValue(newCollectorValue);

                if (!_collectorValueRepository.Save())
                {
                    throw new Exception($"Upserting collector value {id} failed on save.");
                }

                var returnedCollectorValue = Mapper.Map<CollectorValueDto>(newCollectorValue);
                return CreatedAtRoute("GetCollectorValue", 
                    new { id = returnedCollectorValue.Id }, 
                    returnedCollectorValue);
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
                var collectorValueDto = new CollectorValueUpdateDto();
                patchDoc.ApplyTo(collectorValueDto);

                var newCollectorValue = Mapper.Map<CollectorValue>(collectorValueDto);
                newCollectorValue.Id = id;

                _collectorValueRepository.AddCollectorValue(newCollectorValue);

                if (!_collectorValueRepository.Save())
                {
                    throw new Exception($"Upserting collector value {id} failed on save.");
                }

                var returnedCollectorValue = Mapper.Map<CollectorValueDto>(newCollectorValue);
                return CreatedAtRoute("GetCollectorValue",
                    new { id = returnedCollectorValue.Id },
                    returnedCollectorValue);
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
    }
}