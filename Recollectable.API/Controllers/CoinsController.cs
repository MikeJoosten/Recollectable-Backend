using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/coins")]
    public class CoinsController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IControllerService _controllerService;

        public CoinsController(IUnitOfWork unitOfWork,
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetCoins")]
        public IActionResult GetCoins(CurrenciesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<CoinDto, Coin>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<CoinDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var coinsFromRepo = _unitOfWork.CoinRepository.Get(resourceParameters);
            var coins = _controllerService.Mapper.Map<IEnumerable<CoinDto>>(coinsFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = coinsFromRepo.TotalCount,
                    pageSize = coinsFromRepo.PageSize,
                    currentPage = coinsFromRepo.CurrentPage,
                    totalPages = coinsFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateCoinsLinks(resourceParameters,
                    coinsFromRepo.HasNext, coinsFromRepo.HasPrevious);
                var shapedCoins = coins.ShapeData(resourceParameters.Fields);

                var linkedCoins = shapedCoins.Select(coin =>
                {
                    var coinAsDictionary = coin as IDictionary<string, object>;
                    var coinLinks = CreateCoinLinks((Guid)coinAsDictionary["Id"],
                        resourceParameters.Fields);

                    coinAsDictionary.Add("links", coinLinks);

                    return coinAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = linkedCoins,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else if (mediaType == "application/json")
            {
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
        public IActionResult GetCoin(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.TypeHelperService.TypeHasProperties<CoinDto>(fields))
            {
                return BadRequest();
            }

            var coinFromRepo = _unitOfWork.CoinRepository.GetById(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            var coin = _controllerService.Mapper.Map<CoinDto>(coinFromRepo);

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
        public IActionResult CreateCoin([FromBody] CoinCreationDto coin,
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

            var country = _unitOfWork.CountryRepository.GetById(coin.CountryId);

            if (country != null && coin.Country == null)
            {
                coin.Country = country;
            }
            else if (coin.CountryId != Guid.Empty || 
                coin.Country.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var collectorValue = _unitOfWork.CollectorValueRepository
                .GetById(coin.CollectorValueId);

            if (collectorValue != null && coin.CollectorValue == null)
            {
                coin.CollectorValue = collectorValue;
            }
            else if (coin.CollectorValueId != Guid.Empty || 
                coin.CollectorValue.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var newCoin = _controllerService.Mapper.Map<Coin>(coin);
            _unitOfWork.CoinRepository.Add(newCoin);

            if (!_unitOfWork.Save())
            {
                throw new Exception("Creating a coin failed on save.");
            }

            var returnedCoin = _controllerService.Mapper.Map<CoinDto>(newCoin);

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
        public IActionResult BlockCoinCreation(Guid id)
        {
            if (_unitOfWork.CoinRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateCoin")]
        public IActionResult UpdateCoin(Guid id, [FromBody] CoinUpdateDto coin)
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

            if (!_unitOfWork.CountryRepository.Exists(coin.CountryId))
            {
                return BadRequest();
            }

            if (!_unitOfWork.CollectorValueRepository.Exists(coin.CollectorValueId))
            {
                return BadRequest();
            }

            var coinFromRepo = _unitOfWork.CoinRepository.GetById(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            coinFromRepo.CountryId = coin.CountryId;
            coinFromRepo.CollectorValueId = coin.CollectorValueId;

            _controllerService.Mapper.Map(coin, coinFromRepo);
            _unitOfWork.CoinRepository.Update(coinFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Updating coin {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCoin")]
        public IActionResult PartiallyUpdateCoin(Guid id,
            [FromBody] JsonPatchDocument<CoinUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var coinFromRepo = _unitOfWork.CoinRepository.GetById(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            var patchedCoin = _controllerService.Mapper.Map<CoinUpdateDto>(coinFromRepo);
            patchDoc.ApplyTo(patchedCoin, ModelState);

            if (patchedCoin.Note == patchedCoin.Subject)
            {
                ModelState.AddModelError(nameof(CoinUpdateDto),
                    "The provided note should be different from the coin's subject");
            }

            TryValidateModel(patchedCoin);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_unitOfWork.CountryRepository.Exists(patchedCoin.CountryId))
            {
                return BadRequest();
            }

            if (!_unitOfWork.CollectorValueRepository.Exists(patchedCoin.CollectorValueId))
            {
                return BadRequest();
            }

            coinFromRepo.CountryId = patchedCoin.CountryId;
            coinFromRepo.CollectorValueId = patchedCoin.CollectorValueId;

            _controllerService.Mapper.Map(patchedCoin, coinFromRepo);
            _unitOfWork.CoinRepository.Update(coinFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching coin {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCoin")]
        public IActionResult DeleteCoin(Guid id)
        {
            var coinFromRepo = _unitOfWork.CoinRepository.GetById(id);

            if (coinFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.CoinRepository.Delete(coinFromRepo);

            if (!_unitOfWork.Save())
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