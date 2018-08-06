using AutoMapper;
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

        public CoinsController(ICoinRepository coinRepository, 
            ICountryRepository countryRepository)
        {
            _coinRepository = coinRepository;
            _countryRepository = countryRepository;
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
            if (coin == null || !_countryRepository.CountryExists(coin.CountryId))
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
    }
}