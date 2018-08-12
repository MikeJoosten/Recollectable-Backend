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
    [Route("api/coins")]
    public class CoinsController : Controller
    {
        private ICoinRepository _coinRepository;
        private ICountryRepository _countryRepository;
        private ICollectorValueRepository _collectorValueRepository;

        public CoinsController(ICoinRepository coinRepository, 
            ICollectorValueRepository collectorValueRepository, 
            ICountryRepository countryRepository)
        {
            _coinRepository = coinRepository;
            _countryRepository = countryRepository;
            _collectorValueRepository = collectorValueRepository;
        }

        [HttpGet]
        public IActionResult GetCoins()
        {
            var coinsFromRepo = _coinRepository.GetCoins();
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
    }
}