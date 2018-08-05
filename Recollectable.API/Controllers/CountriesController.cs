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
    [Route("api/countries")]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public IActionResult GetCountries()
        {
            var countriesFromRepo = _countryRepository.GetCountries();
            var countries = Mapper.Map<IEnumerable<CountryDto>>(countriesFromRepo);
            return Ok(countries);
        }

        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(Guid id)
        {
            var countryFromRepo = _countryRepository.GetCountry(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            var country = Mapper.Map<CountryDto>(countryFromRepo);
            return Ok(country);
        }

        [HttpPost]
        public IActionResult CreateCountry([FromBody] CountryCreationDto country)
        {
            if (country == null)
            {
                return BadRequest();
            }

            var newCountry = Mapper.Map<Country>(country);
            _countryRepository.AddCountry(newCountry);

            if (!_countryRepository.Save())
            {
                throw new Exception("Creating a country failed on save.");
            }

            var returnedCountry = Mapper.Map<CountryDto>(newCountry);
            return CreatedAtRoute("GetCountry", new { id = returnedCountry.Id }, returnedCountry);
        }

        [HttpPost("{id}")]
        public IActionResult BlockCountryCreation(Guid id)
        {
            if (_countryRepository.CountryExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCountry(Guid id, [FromBody] CountryUpdateDto country)
        {
            if (country == null)
            {
                return BadRequest();
            }

            var countryFromRepo = _countryRepository.GetCountry(id);

            if (countryFromRepo == null)
            {
                var newCountry = Mapper.Map<Country>(country);
                newCountry.Id = id;

                _countryRepository.AddCountry(newCountry);

                if (!_countryRepository.Save())
                {
                    throw new Exception($"Upserting country {id} failed on save.");
                }

                var returnedCountry = Mapper.Map<CountryDto>(newCountry);
                return CreatedAtRoute("GetCountry", new { id = returnedCountry.Id }, returnedCountry);
            }

            Mapper.Map(country, countryFromRepo);
            _countryRepository.UpdateCountry(countryFromRepo);

            if (!_countryRepository.Save())
            {
                throw new Exception($"Updating country {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateCountry(Guid id, 
            [FromBody] JsonPatchDocument<CountryUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var countryFromRepo = _countryRepository.GetCountry(id);

            if (countryFromRepo == null)
            {
                var countryDto = new CountryUpdateDto();
                patchDoc.ApplyTo(countryDto);

                var newCountry = Mapper.Map<Country>(countryDto);
                newCountry.Id = id;

                _countryRepository.AddCountry(newCountry);

                if (!_countryRepository.Save())
                {
                    throw new Exception($"Upserting country {id} failed on save.");
                }

                var returnedCountry = Mapper.Map<CountryDto>(newCountry);
                return CreatedAtRoute("GetCountry", new { id = returnedCountry.Id }, returnedCountry);
            }

            var patchedCountry = Mapper.Map<CountryUpdateDto>(countryFromRepo);
            patchDoc.ApplyTo(patchedCountry);

            Mapper.Map(patchedCountry, countryFromRepo);
            _countryRepository.UpdateCountry(countryFromRepo);

            if (!_countryRepository.Save())
            {
                throw new Exception($"Patching country {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCountry(Guid id)
        {
            var countryFromRepo = _countryRepository.GetCountry(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            _countryRepository.DeleteCountry(countryFromRepo);
            
            if (!_countryRepository.Save())
            {
                throw new Exception($"Deleting country {id} failed on save.");
            }

            return NoContent();
        }
    }
}