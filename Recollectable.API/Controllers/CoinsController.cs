using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Helpers;
using Recollectable.API.Models;
using Recollectable.Data.Helpers;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    [Route("api/coins")]
    public class CoinsController : Controller
    {
        private ICoinRepository _coinRepository;
        private ICountryRepository _countryRepository;
        private ICollectorValueRepository _collectorValueRepository;
        private IUrlHelper _urlHelper;

        public CoinsController(ICoinRepository coinRepository, 
            ICollectorValueRepository collectorValueRepository, 
            ICountryRepository countryRepository, IUrlHelper urlHelper)
        {
            _coinRepository = coinRepository;
            _countryRepository = countryRepository;
            _collectorValueRepository = collectorValueRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetCoins")]
        public IActionResult GetCoins(CollectablesResourceParameters resourceParameters)
        {
            var coinsFromRepo = _coinRepository.GetCoins(resourceParameters);

            var previousPageLink = coinsFromRepo.HasPrevious ?
                CreateCoinsResourceUri(resourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = coinsFromRepo.HasNext ?
                CreateCoinsResourceUri(resourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = coinsFromRepo.TotalCount,
                pageSize = coinsFromRepo.PageSize,
                currentPage = coinsFromRepo.CurrentPage,
                totalPages = coinsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var coins = Mapper.Map<IEnumerable<CoinDto>>(coinsFromRepo);
            return Ok(coins);
        }

        [HttpGet("{id}", Name = "GetCoin")]
        public IActionResult GetCoin(Guid id)
        {
            var coinFromRepo = _coinRepository.GetCoin(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            var coin = Mapper.Map<CoinDto>(coinFromRepo);
            return Ok(coin);
        }

        [HttpPost]
        public IActionResult CreateCoin([FromBody] CoinCreationDto coin)
        {
            if (coin == null)
            {
                return BadRequest();
            }

            var country = _countryRepository.GetCountry(coin.CountryId);

            if (country != null && coin.Country == null)
            {
                coin.Country = country;
            }
            else if (coin.CountryId != Guid.Empty || 
                coin.Country.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var collectorValue = _collectorValueRepository
                .GetCollectorValue(coin.CollectorValueId);

            if (collectorValue != null && coin.CollectorValue == null)
            {
                coin.CollectorValue = collectorValue;
            }
            else if (coin.CollectorValueId != Guid.Empty || 
                coin.CollectorValue.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var newCoin = Mapper.Map<Coin>(coin);
            _coinRepository.AddCoin(newCoin);

            if (!_coinRepository.Save())
            {
                throw new Exception("Creating a coin failed on save.");
            }

            var returnedCoin = Mapper.Map<CoinDto>(newCoin);
            return CreatedAtRoute("GetCoin", new { id = returnedCoin.Id }, returnedCoin);
        }

        [HttpPost("{id}")]
        public IActionResult BlockCoinCreation(Guid id)
        {
            if (_coinRepository.CoinExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCoin(Guid id, [FromBody] CoinUpdateDto coin)
        {
            if (coin == null)
            {
                return BadRequest();
            }

            if (!_countryRepository.CountryExists(coin.CountryId))
            {
                return BadRequest();
            }

            if (!_collectorValueRepository.CollectorValueExists(coin.CollectorValueId))
            {
                return BadRequest();
            }

            var coinFromRepo = _coinRepository.GetCoin(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            coinFromRepo.CountryId = coin.CountryId;
            coinFromRepo.CollectorValueId = coin.CollectorValueId;

            Mapper.Map(coin, coinFromRepo);
            _coinRepository.UpdateCoin(coinFromRepo);

            if (!_coinRepository.Save())
            {
                throw new Exception($"Updating coin {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateCoin(Guid id,
            [FromBody] JsonPatchDocument<CoinUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var coinFromRepo = _coinRepository.GetCoin(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            var patchedCoin = Mapper.Map<CoinUpdateDto>(coinFromRepo);
            patchDoc.ApplyTo(patchedCoin);

            if (!_countryRepository.CountryExists(patchedCoin.CountryId))
            {
                return BadRequest();
            }

            if (!_collectorValueRepository.CollectorValueExists(patchedCoin.CollectorValueId))
            {
                return BadRequest();
            }

            coinFromRepo.CountryId = patchedCoin.CountryId;
            coinFromRepo.CollectorValueId = patchedCoin.CollectorValueId;

            Mapper.Map(patchedCoin, coinFromRepo);
            _coinRepository.UpdateCoin(coinFromRepo);

            if (!_coinRepository.Save())
            {
                throw new Exception($"Patching coin {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCoin(Guid id)
        {
            var coinFromRepo = _coinRepository.GetCoin(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            _coinRepository.DeleteCoin(coinFromRepo);

            if (!_coinRepository.Save())
            {
                throw new Exception($"Deleting coin {id} failed on save.");
            }

            return NoContent();
        }

        private string CreateCoinsResourceUri(CollectablesResourceParameters resourceParameters, 
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCoins", new
                    {
                        type = resourceParameters.Type,
                        country = resourceParameters.Country,
                        releaseDate = resourceParameters.ReleaseDate,
                        page = resourceParameters.Page - 1,
                        pageSize = resourceParameters.PageSize
                    });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCoins", new
                    {
                        type = resourceParameters.Type,
                        country = resourceParameters.Country,
                        releaseDate = resourceParameters.ReleaseDate,
                        page = resourceParameters.Page + 1,
                        pageSize = resourceParameters.PageSize
                    });
                default:
                    return _urlHelper.Link("GetCoins", new
                    {
                        type = resourceParameters.Type,
                        country = resourceParameters.Country,
                        releaseDate = resourceParameters.ReleaseDate,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    });
            }
        }
    }
}