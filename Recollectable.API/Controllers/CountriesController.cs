using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Models.Locations;
using Recollectable.Core.Entities.Locations;
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
    //TODO Add Authorization
    [ApiVersion("1.0")]
    [Route("api/countries")]
    public class CountriesController : Controller
    {
        private ICountryService _countryService;
        private IMapper _mapper;

        public CountriesController(ICountryService countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        [HttpHead]
        [HttpGet(Name = "GetCountries")]
        public async Task<IActionResult> GetCountries(CountriesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<Country>(resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<CountryDto>(resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedCountries = await _countryService.FindCountries(resourceParameters);
            var countries = _mapper.Map<IEnumerable<CountryDto>>(retrievedCountries);
            var shapedCountries = countries.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var paginationMetadata = new
                {
                    totalCount = retrievedCountries.TotalCount,
                    pageSize = retrievedCountries.PageSize,
                    currentPage = retrievedCountries.CurrentPage,
                    totalPages = retrievedCountries.TotalPages
                };

                Response.Headers.Add("X-Pagination", 
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCountriesLinks(resourceParameters,
                    retrievedCountries.HasNext, retrievedCountries.HasPrevious);

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
            else
            {
                var previousPageLink = retrievedCountries.HasPrevious ?
                    CreateCountriesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedCountries.HasNext ?
                    CreateCountriesResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedCountries.TotalCount,
                    pageSize = retrievedCountries.PageSize,
                    currentPage = retrievedCountries.CurrentPage,
                    totalPages = retrievedCountries.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(shapedCountries);
            }
        }

        [HttpGet("{id}", Name = "GetCountry")]
        public async Task<IActionResult> GetCountry(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!TypeHelper.TypeHasProperties<CountryDto>(fields))
            {
                return BadRequest();
            }

            var retrievedCountry = await _countryService.FindCountryById(id);

            if (retrievedCountry == null)
            {
                return NotFound();
            }

            var country = _mapper.Map<CountryDto>(retrievedCountry);
            var shapedCountry = country.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateCountryLinks(id, fields);
                var linkedResource = shapedCountry as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedCountry);
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
            await _countryService.CreateCountry(newCountry);

            if (!await _countryService.Save())
            {
                throw new Exception("Creating a country failed on save.");
            }

            var returnedCountry = _mapper.Map<CountryDto>(newCountry);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCountryLinks(returnedCountry.Id, null);
                var linkedResource = returnedCountry.ShapeData(null) as IDictionary<string, object>;

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
            if (await _countryService.CountryExists(id))
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

            var retrievedCountry = await _countryService.FindCountryById(id);

            if (retrievedCountry == null)
            {
                return NotFound();
            }

            _mapper.Map(country, retrievedCountry);
            _countryService.UpdateCountry(retrievedCountry);

            if (!await _countryService.Save())
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

            var retrievedCountry = await _countryService.FindCountryById(id);

            if (retrievedCountry == null)
            {
                return NotFound();
            }

            var patchedCountry = _mapper.Map<CountryUpdateDto>(retrievedCountry);
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

            _mapper.Map(patchedCountry, retrievedCountry);
            _countryService.UpdateCountry(retrievedCountry);

            if (!await _countryService.Save())
            {
                throw new Exception($"Patching country {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCountry")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var retrievedCountry = await _countryService.FindCountryById(id);

            if (retrievedCountry == null)
            {
                return NotFound();
            }

            _countryService.RemoveCountry(retrievedCountry);
            
            if (!await _countryService.Save())
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