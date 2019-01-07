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

        /// <summary>
        /// Retrieves countries
        /// </summary>
        /// <returns>List of countries</returns>
        /// <response code="200">Returns a list of countries</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetCountries")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CountryDto), 200)]
        [ProducesResponseType(400)]
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
                if (!string.IsNullOrEmpty(resourceParameters.Fields) &&
                    !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
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

        /// <summary>
        /// Retrieves the requested country by country ID
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested country</returns>
        /// <response code="200">Returns the requested country</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting country ID</response>
        [HttpGet("{id}", Name = "GetCountry")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CountryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
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

        /// <summary>
        /// Creates a country
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /countries
        ///     {
        ///         "name": "Cuba",
        ///         "description": "A Caribbean island near the Florida coast"
        ///     }
        /// </remarks>
        /// <param name="country">Custom country</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created country</returns>
        /// <response code="201">Returns the newly created country</response>
        /// <response code="400">Invalid country</response>
        /// <response code="422">Invalid country validation</response>
        [HttpPost(Name = "CreateCountry")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CountryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
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

        /// <summary>
        /// Invalid country creation request
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <response code="404">Unexisting country ID</response>
        /// <response code="409">Already existing country ID</response>
        [HttpPost("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockCountryCreation(Guid id)
        {
            if (await _countryService.CountryExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        /// <summary>
        /// Updates a country
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /countries/{id}
        ///     {
        ///         "name": "Thailand",
        ///         "description": "Previously known as Siam, this country is located in Southeast Asia"
        ///     }
        /// </remarks>
        /// <param name="id">Country ID</param>
        /// <param name="country">Custom country</param>
        /// <response code="204">Updated the country successfully</response>
        /// <response code="400">Invalid country</response>
        /// <response code="404">Unexisting country ID</response>
        /// <response code="422">Invalid country validation</response>
        [HttpPut("{id}", Name = "UpdateCountry")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
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

        /// <summary>
        /// Update specific fields of a country
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /countries/{id}
        ///     [
        ///	        { "op": "replace", "path": "/name", "value": "Germany" },
        ///	        { "op": "remove", "path": "/description" } 
        ///     ]
        /// </remarks>
        /// <param name="id">Country ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        /// <response code="204">Updated the country successfully</response>
        /// <response code="400">Invalid patch document</response>
        /// <response code="404">Unexisting country ID</response>
        /// <response code="422">Invalid country validation</response>
        [HttpPatch("{id}", Name = "PartiallyUpdateCountry")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
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

        /// <summary>
        /// Removes a country
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <response code="204">Removed the country successfully</response>
        /// <response code="404">Unexisting country ID</response>
        [HttpDelete("{id}", Name = "DeleteCountry")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
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