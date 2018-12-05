﻿using AutoMapper;
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

        [HttpHead]
        [HttpGet(Name = "GetCoins")]
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

            if (mediaType == "application/json+hateoas")
            {
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
                var shapedCoins = coins.ShapeData(resourceParameters.Fields);

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
            else if (mediaType == "application/json")
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

                return Ok(coins.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(coins);
            }
        }

        [HttpGet("{id}", Name = "GetCoin")]
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

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateCoinLinks(id, fields);
                var linkedResource = coin.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(coin.ShapeData(fields));
            }
            else
            {
                return Ok(coin);
            }
        }

        [HttpPost(Name = "CreateCoin")]
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
                var linkedResource = returnedCoin.ShapeData(null)
                    as IDictionary<string, object>;

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

        [HttpPost("{id}")]
        public async Task<IActionResult> BlockCoinCreation(Guid id)
        {
            if (await _coinService.CoinExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCoin")]
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

        [HttpPatch("{id}", Name = "PartiallyUpdateCoin")]
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

        [HttpDelete("{id}", Name = "DeleteCoin")]
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

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(Url.Link("GetCoins",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(Url.Link("CreateCoins",
                    new { }), "create_coins", "POST"));

                links.Add(new LinkDto(Url.Link("UpdateCoins",
                    new { id }), "update_coins", "PUT"));

                links.Add(new LinkDto(Url.Link("PartiallyUpdateCoins",
                    new { id }), "partially_update_coins", "PATCH"));

                links.Add(new LinkDto(Url.Link("DeleteCoins",
                    new { id }), "delete_coins", "DELETE"));
            }

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