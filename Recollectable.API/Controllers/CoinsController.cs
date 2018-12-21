using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
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
    [Route("api/coins")]
    public class CoinsController : Controller
    {
        private ICoinService _coinService;
        private ICountryService _countryService;
        private ICollectorValueService _collectorValueService;
        private IMapper _mapper;

        public CoinsController(ICoinService coinService, ICountryService countryService,
            ICollectorValueService collectorValueService, IMapper mapper)
        {
            _coinService = coinService;
            _countryService = countryService;
            _collectorValueService = collectorValueService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves coins
        /// </summary>
        /// <returns>List of coins</returns>
        /// <response code="200">Returns a list of coins</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetCoins")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CoinDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCoins(CurrenciesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<Coin>(resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!TypeHelper.TypeHasProperties<CoinDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var retrievedCoins = await _coinService.FindCoins(resourceParameters);
            var coins = _mapper.Map<IEnumerable<CoinDto>>(retrievedCoins);
            var shapedCoins = coins.ShapeData(resourceParameters.Fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(resourceParameters.Fields) &&
                    !resourceParameters.Fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var paginationMetadata = new
                {
                    totalCount = retrievedCoins.TotalCount,
                    pageSize = retrievedCoins.PageSize,
                    currentPage = retrievedCoins.CurrentPage,
                    totalPages = retrievedCoins.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCoinsLinks(resourceParameters,
                    retrievedCoins.HasNext, retrievedCoins.HasPrevious);

                var linkedCoins = shapedCoins.Select(coin =>
                {
                    var coinAsDictionary = coin as IDictionary<string, object>;
                    var coinLinks = CreateCoinLinks((Guid)coinAsDictionary["Id"],
                        resourceParameters.Fields);

                    coinAsDictionary.Add("links", coinLinks);

                    return coinAsDictionary;
                });

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedCoins,
                    Links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = retrievedCoins.HasPrevious ?
                    CreateCoinsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = retrievedCoins.HasNext ?
                    CreateCoinsResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = retrievedCoins.TotalCount,
                    pageSize = retrievedCoins.PageSize,
                    currentPage = retrievedCoins.CurrentPage,
                    totalPages = retrievedCoins.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(shapedCoins);
            }
        }

        /// <summary>
        /// Retrieves the requested coin by coin ID
        /// </summary>
        /// <param name="id">Coin ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested coin</returns>
        /// <response code="200">Returns the requested coin</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting coin ID</response>
        [HttpGet("{id}", Name = "GetCoin")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CoinDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCoin(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!TypeHelper.TypeHasProperties<CoinDto>(fields))
            {
                return BadRequest();
            }

            var retrievedCoin = await _coinService.FindCoinById(id);

            if (retrievedCoin == null)
            {
                return NotFound();
            }

            var coin = _mapper.Map<CoinDto>(retrievedCoin);
            var shapedCoin = coin.ShapeData(fields);

            if (mediaType == "application/json+hateoas")
            {
                if (!string.IsNullOrEmpty(fields) && !fields.ToLowerInvariant().Contains("id"))
                {
                    return BadRequest("Field parameter 'id' is required");
                }

                var links = CreateCoinLinks(id, fields);
                var linkedResource = shapedCoin as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else
            {
                return Ok(shapedCoin);
            }
        }

        //TODO Add Sample request
        /// <summary>
        /// Creates a coin
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /coins
        ///     {
        ///         
        ///     }
        /// </remarks>
        /// <param name="coin">Custom coin</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created coin</returns>
        /// <response code="201">Returns the newly created coin</response>
        /// <response code="400">Invalid coin</response>
        /// <response code="422">Invalid coin validation</response>
        [HttpPost(Name = "CreateCoin")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(CoinDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCoin([FromBody] CoinCreationDto coin,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (coin == null)
            {
                return BadRequest();
            }

            if ((coin.Note == coin.Subject) && (coin.Note != null || coin.Subject != null))
            {
                ModelState.AddModelError(nameof(CoinCreationDto),
                    "The provided note should be different from the coin's subject");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryService.CountryExists(coin.CountryId))
            {
                return BadRequest();
            }

            var newCoin = _mapper.Map<Coin>(coin);

            var existingCollectorValue = await _collectorValueService.FindCollectorValueByValues(newCoin.CollectorValue);
            newCoin.CollectorValueId = existingCollectorValue == null ? Guid.NewGuid() : existingCollectorValue.Id;

            await _coinService.CreateCoin(newCoin);

            if (!await _coinService.Save())
            {
                throw new Exception("Creating a coin failed on save.");
            }

            var returnedCoin = _mapper.Map<CoinDto>(newCoin);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCoinLinks(returnedCoin.Id, null);
                var linkedResource = returnedCoin.ShapeData(null) as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetCoin", 
                    new { id = returnedCoin.Id }, 
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetCoin",
                    new { id = returnedCoin.Id },
                    returnedCoin);
            }
        }

        /// <summary>
        /// Invalid coin creation request
        /// </summary>
        /// <param name="id">Coin ID</param>
        /// <response code="404">Unexisting coin ID</response>
        /// <response code="409">Already existing coin ID</response>
        [HttpPost("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockCoinCreation(Guid id)
        {
            if (await _coinService.CoinExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        //TODO Add Sample request
        /// <summary>
        /// Updates a coin
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /coins/{id}
        ///     {
        ///         
        ///     }
        /// </remarks>
        /// <param name="id">Coin ID</param>
        /// <param name="coin">Custom coin</param>
        /// <response code="204">Updated the coin successfully</response>
        /// <response code="400">Invalid coin</response>
        /// <response code="404">Unexisting coin ID</response>
        /// <response code="422">Invalid coin validation</response>
        [HttpPut("{id}", Name = "UpdateCoin")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateCoin(Guid id, [FromBody] CoinUpdateDto coin)
        {
            if (coin == null)
            {
                return BadRequest();
            }

            if ((coin.Note == coin.Subject) && (coin.Note != null || coin.Subject != null))
            {
                ModelState.AddModelError(nameof(CoinUpdateDto),
                    "The provided note should be different from the coin's subject");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryService.CountryExists(coin.CountryId))
            {
                return BadRequest();
            }

            var retrievedCoin = await _coinService.FindCoinById(id);

            if (retrievedCoin == null)
            {
                return NotFound();
            }

            var collectorValue = _mapper.Map<CollectorValue>(coin.CollectorValue);
            var existingCollectorValue = await _collectorValueService.FindCollectorValueByValues(collectorValue);
            retrievedCoin.CollectorValueId = existingCollectorValue == null ? Guid.NewGuid() : existingCollectorValue.Id;
            retrievedCoin.CollectorValue = collectorValue;

            _mapper.Map(coin, retrievedCoin);
            _coinService.UpdateCoin(retrievedCoin);

            if (!await _coinService.Save())
            {
                throw new Exception($"Updating coin {id} failed on save.");
            }

            return NoContent();
        }

        //TODO Add Sample request
        /// <summary>
        /// Update specific fields of a coin
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /coins/{id}
        ///     [
        ///	        
        ///     ]
        /// </remarks>
        /// <param name="id">Coin ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        /// <response code="204">Updated the coin successfully</response>
        /// <response code="400">Invalid patch document</response>
        /// <response code="404">Unexisting coin ID</response>
        /// <response code="422">Invalid coin validation</response>
        [HttpPatch("{id}", Name = "PartiallyUpdateCoin")]
        [Consumes("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PartiallyUpdateCoin(Guid id,
            [FromBody] JsonPatchDocument<CoinUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var retrievedCoin = await _coinService.FindCoinById(id);

            if (retrievedCoin == null)
            {
                return NotFound();
            }

            var patchedCoin = _mapper.Map<CoinUpdateDto>(retrievedCoin);
            patchDoc.ApplyTo(patchedCoin, ModelState);

            if (patchedCoin.Note?.ToLowerInvariant() == patchedCoin.Subject?.ToLowerInvariant())
            {
                ModelState.AddModelError(nameof(CoinUpdateDto),
                    "The provided note should be different from the coin's subject");
            }

            TryValidateModel(patchedCoin);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryService.CountryExists(patchedCoin.CountryId))
            {
                return BadRequest();
            }

            var collectorValue = _mapper.Map<CollectorValue>(patchedCoin.CollectorValue);
            var existingCollectorValue = await _collectorValueService.FindCollectorValueByValues(collectorValue);
            retrievedCoin.CollectorValueId = existingCollectorValue == null ? Guid.NewGuid() : existingCollectorValue.Id;
            retrievedCoin.CollectorValue = collectorValue;

            _mapper.Map(patchedCoin, retrievedCoin);
            _coinService.UpdateCoin(retrievedCoin);

            if (!await _coinService.Save())
            {
                throw new Exception($"Patching coin {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Removes a coin
        /// </summary>
        /// <param name="id">Coin ID</param>
        /// <response code="204">Removed the coin successfully</response>
        /// <response code="404">Unexisting coin ID</response>
        [HttpDelete("{id}", Name = "DeleteCoin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCoin(Guid id)
        {
            var retrievedCoin = await _coinService.FindCoinById(id);

            if (retrievedCoin == null)
            {
                return NotFound();
            }

            _coinService.RemoveCoin(retrievedCoin);

            if (!await _coinService.Save())
            {
                throw new Exception($"Deleting coin {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCoinsOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateCoinsResourceUri(CurrenciesResourceParameters resourceParameters, 
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCoins", new
                    {
                        type = resourceParameters.Type,
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCoins", new
                    {
                        type = resourceParameters.Type,
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return Url.Link("GetCoins", new
                    {
                        type = resourceParameters.Type,
                        country = resourceParameters.Country,
                        search = resourceParameters.Search,
                        orderBy = resourceParameters.OrderBy,
                        fields = resourceParameters.Fields,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateCoinLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("GetCoins",
                new { id }), "self", "GET"));

            links.Add(new LinkDto(Url.Link("CreateCoin",
                new { }), "create_coins", "POST"));

            links.Add(new LinkDto(Url.Link("UpdateCoin",
                new { id }), "update_coins", "PUT"));

            links.Add(new LinkDto(Url.Link("PartiallyUpdateCoin",
                new { id }), "partially_update_coins", "PATCH"));

            links.Add(new LinkDto(Url.Link("DeleteCoin",
                new { id }), "delete_coins", "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateCoinsLinks
            (CurrenciesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateCoinsResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCoinsResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCoinsResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}