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
    [Route("api/conditions")]
    public class ConditionsController : Controller
    {
        private IConditionRepository _conditionRepository;

        public ConditionsController(IConditionRepository conditionRepository)
        {
            _conditionRepository = conditionRepository;
        }

        [HttpGet]
        public IActionResult GetConditions()
        {
            var conditionsFromRepo = _conditionRepository.GetConditions();
            var conditions = Mapper.Map<IEnumerable<ConditionDto>>(conditionsFromRepo);
            return Ok(conditions);
        }

        [HttpGet("{id}", Name = "GetCondition")]
        public IActionResult GetCondition(Guid id)
        {
            var conditionFromRepo = _conditionRepository.GetCondition(id);

            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            var condition = Mapper.Map<ConditionDto>(conditionFromRepo);
            return Ok(condition);
        }

        [HttpPost]
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
            return CreatedAtRoute("GetCondition",
                new { id = returnedCondition.Id },
                returnedCondition);
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

        [HttpPut("{id}")]
        public IActionResult UpdateCondition(Guid id, [FromBody] ConditionUpdateDto condition)
        {
            if (condition == null)
            {
                return BadRequest();
            }

            var conditionFromRepo = _conditionRepository.GetCondition(id);

            if (conditionFromRepo == null)
            {
                var newCondition = Mapper.Map<Condition>(condition);
                newCondition.Id = id;

                _conditionRepository.AddCondition(newCondition);

                if (!_conditionRepository.Save())
                {
                    throw new Exception($"Upserting condition {id} failed on save.");
                }

                var returnedCondition = Mapper.Map<ConditionDto>(newCondition);
                return CreatedAtRoute("GetCondition",
                    new { id = returnedCondition.Id },
                    returnedCondition);
            }

            Mapper.Map(condition, conditionFromRepo);
            _conditionRepository.UpdateCondition(conditionFromRepo);

            if (!_conditionRepository.Save())
            {
                throw new Exception($"Updating condition {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
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
                var conditionDto = new ConditionUpdateDto();
                patchDoc.ApplyTo(conditionDto);

                var newCondition = Mapper.Map<Condition>(conditionDto);
                newCondition.Id = id;

                _conditionRepository.AddCondition(newCondition);

                if (!_conditionRepository.Save())
                {
                    throw new Exception($"Upserting condition {id} failed on save.");
                }

                var returnedCondition = Mapper.Map<ConditionDto>(newCondition);
                return CreatedAtRoute("GetCondition",
                    new { id = returnedCondition.Id },
                    returnedCondition);
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

        [HttpDelete("{id}")]
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
    }
}