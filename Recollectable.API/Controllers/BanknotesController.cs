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
    [Route("api/banknotes")]
    public class BanknotesController : Controller
    {
        private IBanknoteRepository _banknoteRepository;
        private ICountryRepository _countryRepository;
        private ICollectorValueRepository _collectorValueRepository;

        public BanknotesController(IBanknoteRepository banknoteRepository,
            ICollectorValueRepository collectorValueRepository,
            ICountryRepository countryRepository)
        {
            _banknoteRepository = banknoteRepository;
            _countryRepository = countryRepository;
            _collectorValueRepository = collectorValueRepository;
        }

        [HttpGet]
        public IActionResult GetBanknotes()
        {
            var banknotesFromRepo = _banknoteRepository.GetBanknotes();
            var banknotes = Mapper.Map<IEnumerable<BanknoteDto>>(banknotesFromRepo);
            return Ok(banknotes);
        }

        [HttpGet("{id}", Name = "GetBanknote")]
        public IActionResult GetBanknote(Guid id)
        {
            var banknoteFromRepo = _banknoteRepository.GetBanknote(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var banknote = Mapper.Map<BanknoteDto>(banknoteFromRepo);
            return Ok(banknote);
        }

        [HttpPost]
        public IActionResult CreateBanknote([FromBody] BanknoteCreationDto banknote)
        {
            if (banknote == null)
            {
                return BadRequest();
            }

            var country = _countryRepository.GetCountry(banknote.Country.Id);

            if (country != null)
            {
                banknote.Country = country;
            }
            else if (banknote.Country.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var collectorValue = _collectorValueRepository
                .GetCollectorValue(banknote.CollectorValue.Id);

            if (collectorValue != null)
            {
                banknote.CollectorValue = collectorValue;
            }
            else if (banknote.CollectorValue.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var newBanknote = Mapper.Map<Banknote>(banknote);
            _banknoteRepository.AddBanknote(newBanknote);

            if (!_banknoteRepository.Save())
            {
                throw new Exception("Creating a banknote failed on save.");
            }

            var returnedBanknote = Mapper.Map<BanknoteDto>(newBanknote);
            return CreatedAtRoute("GetBanknote", 
                new { id = returnedBanknote.Id }, 
                returnedBanknote);
        }

        [HttpPost("{id}")]
        public IActionResult BlockBanknoteCreation(Guid id)
        {
            if (_banknoteRepository.BanknoteExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBanknote(Guid id, [FromBody] BanknoteUpdateDto banknote)
        {
            if (banknote == null)
            {
                return BadRequest();
            }

            if (!_countryRepository.CountryExists(banknote.CountryId))
            {
                return BadRequest();
            }

            if (!_collectorValueRepository.CollectorValueExists(banknote.CollectorValueId))
            {
                return BadRequest();
            }

            var banknoteFromRepo = _banknoteRepository.GetBanknote(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            banknoteFromRepo.CountryId = banknote.CountryId;
            banknoteFromRepo.CollectorValueId = banknote.CollectorValueId;

            Mapper.Map(banknote, banknoteFromRepo);
            _banknoteRepository.UpdateBanknote(banknoteFromRepo);

            if (!_banknoteRepository.Save())
            {
                throw new Exception($"Updating banknote {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBanknote(Guid id,
            [FromBody] JsonPatchDocument<BanknoteUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var banknoteFromRepo = _banknoteRepository.GetBanknote(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var patchedCoin = Mapper.Map<BanknoteUpdateDto>(banknoteFromRepo);
            patchDoc.ApplyTo(patchedCoin);

            var country = _countryRepository.GetCountry(patchedCoin.CountryId);

            if (country == null)
            {
                return BadRequest();
            }

            var collectorValue = _collectorValueRepository
                .GetCollectorValue(patchedCoin.CollectorValueId);

            if (collectorValue == null)
            {
                return BadRequest();
            }

            banknoteFromRepo.Country = country;
            banknoteFromRepo.CollectorValue = collectorValue;

            Mapper.Map(patchedCoin, banknoteFromRepo);
            _banknoteRepository.UpdateBanknote(banknoteFromRepo);

            if (!_banknoteRepository.Save())
            {
                throw new Exception($"Patching banknote {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBanknote(Guid id)
        {
            var banknoteFromRepo = _banknoteRepository.GetBanknote(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            _banknoteRepository.DeleteBanknote(banknoteFromRepo);

            if (!_banknoteRepository.Save())
            {
                throw new Exception($"Deleting banknote {id} failed on save.");
            }

            return NoContent();
        }
    }
}