using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Locations;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    //TODO Add Authorization
    [ApiVersion("1.0")]
    [Route("api/countries")]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IMapper _mapper;

        public CountriesController(ICountryRepository countryRepository, ITypeHelperService typeHelperService,
            IPropertyMappingService propertyMappingService, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetCountries")]
        public async Task<IActionResult> GetCountries(CountriesResourceParameters resourceParameters,
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

            var countriesFromRepo = await _countryRepository.GetCountries(resourceParameters);
            var countries = _mapper.Map<IEnumerable<CountryDto>>(countriesFromRepo);

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

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedCountries,
                    Links = links
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
        public async Task<IActionResult> GetCountry(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<CountryDto>(fields))
            {
                return BadRequest();
            }

            var countryFromRepo = await _countryRepository.GetCountryById(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            var country = _mapper.Map<CountryDto>(countryFromRepo);

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
        public async Task<IActionResult> CreateCountry([FromBody] CountryCreationDto country,
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

            var newCountry = _mapper.Map<Country>(country);
            _countryRepository.AddCountry(newCountry);

            if (!await _countryRepository.Save())
            {
                throw new Exception("Creating a country failed on save.");
            }

            var returnedCountry = _mapper.Map<CountryDto>(newCountry);

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
        public async Task<IActionResult> BlockCountryCreation(Guid id)
        {
            if (await _countryRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCountry")]
        public async Task<IActionResult> UpdateCountry(Guid id, [FromBody] CountryUpdateDto country)
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

            var countryFromRepo = await _countryRepository.GetCountryById(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(country, countryFromRepo);
            _countryRepository.UpdateCountry(countryFromRepo);

            if (!await _countryRepository.Save())
            {
                throw new Exception($"Updating country {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCountry")]
        public async Task<IActionResult> PartiallyUpdateCountry(Guid id, 
            [FromBody] JsonPatchDocument<CountryUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var countryFromRepo = await _countryRepository.GetCountryById(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            var patchedCountry = _mapper.Map<CountryUpdateDto>(countryFromRepo);
            patchDoc.ApplyTo(patchedCountry, ModelState);

            if (patchedCountry.Description?.ToLowerInvariant() == patchedCountry.Name?.ToLowerInvariant())
            {
                ModelState.AddModelError(nameof(CountryUpdateDto),
                    "The provided description should be different from the country name");
            }

            TryValidateModel(patchedCountry);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(patchedCountry, countryFromRepo);
            _countryRepository.UpdateCountry(countryFromRepo);

            if (!await _countryRepository.Save())
            {
                throw new Exception($"Patching country {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCountry")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var countryFromRepo = await _countryRepository.GetCountryById(id);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            _countryRepository.DeleteCountry(countryFromRepo);
            
            if (!await _countryRepository.Save())
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
                    return Url.Link("GetCountries", new
                    {
                        name = resourceParameters.Name,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCountries", new
                    {
                        name = resourceParameters.Name,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetCountries", new
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
                links.Add(new LinkDto(Url.Link("GetCountry",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(Url.Link("CreateCountry",
                    new { }), "create_country", "POST"));

                links.Add(new LinkDto(Url.Link("UpdateCountry",
                    new { id }), "update_country", "PUT"));

                links.Add(new LinkDto(Url.Link("PartiallyUpdateCountry",
                    new { id }), "partially_update_country", "PATCH"));

                links.Add(new LinkDto(Url.Link("DeleteCountry",
                    new { id }), "delete_country", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateCountriesLinks
            (CountriesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateCountriesResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

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