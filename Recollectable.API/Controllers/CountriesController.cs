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
    [Route("api/countries")]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public CountriesController(ICountryRepository countryRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _countryRepository = countryRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetCountries")]
        public IActionResult GetCountries(CountriesResourceParameters resourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CountryDto, Country>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CountryDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var countriesFromRepo = _countryRepository.GetCountries(resourceParameters);

            var previousPageLink = countriesFromRepo.HasPrevious ?
                CreateCountriesResourceUri(resourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = countriesFromRepo.HasNext ?
                CreateCountriesResourceUri(resourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = countriesFromRepo.TotalCount,
                pageSize = countriesFromRepo.PageSize,
                currentPage = countriesFromRepo.CurrentPage,
                totalPages = countriesFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var countries = Mapper.Map<IEnumerable<CountryDto>>(countriesFromRepo);
            return Ok(countries.ShapeData(resourceParameters.Fields));
        }

        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<CountryDto>(fields))
            {
                return BadRequest();
            }

            var countryFromRepo = _countryRepository.GetCountry(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            var country = Mapper.Map<CountryDto>(countryFromRepo);
            return Ok(country.ShapeData(fields));
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
                return NotFound();
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
                return NotFound();
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

        private string CreateCountriesResourceUri(CountriesResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCountries", new
                    {
                        name = resourceParameters.Name,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCountries", new
                    {
                        name = resourceParameters.Name,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetCountries", new
                    {
                        name = resourceParameters.Name,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }
    }
}