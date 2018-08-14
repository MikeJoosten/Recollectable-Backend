using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Helpers;
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

        [HttpHead]
        [HttpGet(Name = "GetCountries")]
        public IActionResult GetCountries(CountriesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
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
            var countries = Mapper.Map<IEnumerable<CountryDto>>(countriesFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = countriesFromRepo.TotalCount,
                    pageSize = countriesFromRepo.PageSize,
                    currentPage = countriesFromRepo.CurrentPage,
                    totalPages = countriesFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCountriesLinks(resourceParameters,
                    countriesFromRepo.HasNext, countriesFromRepo.HasPrevious);
                var shapedCountries = countries.ShapeData(resourceParameters.Fields);

                var linkedCountries = shapedCountries.Select(country =>
                {
                    var countryAsDictionary = country as IDictionary<string, object>;
                    var countryLinks = CreateCountryLinks((Guid)countryAsDictionary["Id"],
                        resourceParameters.Fields);

                    countryAsDictionary.Add("links", countryLinks);

                    return countryAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = linkedCountries,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else if (mediaType == "application/json")
            {
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
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(countries.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(countries);
            }
        }

        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
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

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCountryLinks(id, fields);
                var linkedResource = country.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(country.ShapeData(fields));
            }
            else
            {
                return Ok(country);
            }
        }

        [HttpPost(Name = "CreateCountry")]
        public IActionResult CreateCountry([FromBody] CountryCreationDto country,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (country == null)
            {
                return BadRequest();
            }

            if (country.Description == country.Name)
            {
                ModelState.AddModelError(nameof(CountryCreationDto),
                    "The provided description should be different from the country name");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var newCountry = Mapper.Map<Country>(country);
            _countryRepository.AddCountry(newCountry);

            if (!_countryRepository.Save())
            {
                throw new Exception("Creating a country failed on save.");
            }

            var returnedCountry = Mapper.Map<CountryDto>(newCountry);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCountryLinks(returnedCountry.Id, null);
                var linkedResource = returnedCountry.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCountry", 
                    new { id = returnedCountry.Id }, 
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCountry",
                    new { id = returnedCountry.Id },
                    returnedCountry);
            }
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

        [HttpPut("{id}", Name = "UpdateCountry")]
        public IActionResult UpdateCountry(Guid id, [FromBody] CountryUpdateDto country)
        {
            if (country == null)
            {
                return BadRequest();
            }

            if (country.Description == country.Name)
            {
                ModelState.AddModelError(nameof(CountryUpdateDto),
                    "The provided description should be different from the country name");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
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

        [HttpPatch("{id}", Name = "PartiallyUpdateCountry")]
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
            patchDoc.ApplyTo(patchedCountry, ModelState);

            if (patchedCountry.Description == patchedCountry.Name)
            {
                ModelState.AddModelError(nameof(CountryUpdateDto),
                    "The provided description should be different from the country name");
            }

            TryValidateModel(patchedCountry);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(patchedCountry, countryFromRepo);
            _countryRepository.UpdateCountry(countryFromRepo);

            if (!_countryRepository.Save())
            {
                throw new Exception($"Patching country {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCountry")]
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

        [HttpOptions]
        public IActionResult GetCountriesOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
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

        private IEnumerable<LinkDto> CreateCountryLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(_urlHelper.Link("GetCountry",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("CreateCountry",
                    new { }), "create_country", "POST"));

                links.Add(new LinkDto(_urlHelper.Link("UpdateCountry",
                    new { id }), "update_country", "PUT"));

                links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateCountry",
                    new { id }), "partially_update_country", "PATCH"));

                links.Add(new LinkDto(_urlHelper.Link("DeleteCountry",
                    new { id }), "delete_country", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateCountriesLinks
            (CountriesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCountriesResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCountriesResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCountriesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}