using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.DTOs.Common;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Enums;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.API.Controllers
{
    [Route("api/banknotes")]
    public class BanknotesController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IControllerService _controllerService;

        public BanknotesController(IUnitOfWork unitOfWork, 
            IControllerService controllerService)
        {
            _unitOfWork = unitOfWork;
            _controllerService = controllerService;
        }

        [HttpHead]
        [HttpGet(Name = "GetBanknotes")]
        public IActionResult GetBanknotes(CurrenciesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.PropertyMappingService.ValidMappingExistsFor<BanknoteDto, Banknote>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_controllerService.TypeHelperService.TypeHasProperties<BanknoteDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var banknotesFromRepo = _unitOfWork.BanknoteRepository.Get(resourceParameters);
            var banknotes = Mapper.Map<IEnumerable<BanknoteDto>>(banknotesFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var paginationMetadata = new
                {
                    totalCount = banknotesFromRepo.TotalCount,
                    pageSize = banknotesFromRepo.PageSize,
                    currentPage = banknotesFromRepo.CurrentPage,
                    totalPages = banknotesFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateBanknotesLinks(resourceParameters,
                    banknotesFromRepo.HasNext, banknotesFromRepo.HasPrevious);
                var shapedBanknotes = banknotes.ShapeData(resourceParameters.Fields);

                var linkedBanknotes = shapedBanknotes.Select(banknote =>
                {
                    var banknoteAsDictionary = banknote as IDictionary<string, object>;
                    var banknoteLinks = CreateBanknoteLinks((Guid)banknoteAsDictionary["Id"],
                        resourceParameters.Fields);

                    banknoteAsDictionary.Add("links", banknoteLinks);

                    return banknoteAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = linkedBanknotes,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else if (mediaType == "application/json")
            {
                var previousPageLink = banknotesFromRepo.HasPrevious ?
                    CreateBanknotesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = banknotesFromRepo.HasNext ?
                    CreateBanknotesResourceUri(resourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = banknotesFromRepo.TotalCount,
                    pageSize = banknotesFromRepo.PageSize,
                    currentPage = banknotesFromRepo.CurrentPage,
                    totalPages = banknotesFromRepo.TotalPages,
                    previousPageLink,
                    nextPageLink,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(banknotes.ShapeData(resourceParameters.Fields));
            }
            else
            {
                return Ok(banknotes);
            }
        }

        [HttpGet("{id}", Name = "GetBanknote")]
        public IActionResult GetBanknote(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_controllerService.TypeHelperService.TypeHasProperties<BanknoteDto>(fields))
            {
                return BadRequest();
            }

            var banknoteFromRepo = _unitOfWork.BanknoteRepository.GetById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var banknote = Mapper.Map<BanknoteDto>(banknoteFromRepo);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateBanknoteLinks(id, fields);
                var linkedResource = banknote.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return Ok(linkedResource);
            }
            else if (mediaType == "application/json")
            {
                return Ok(banknote.ShapeData(fields));
            }
            else
            {
                return Ok(banknote);
            }
        }

        [HttpPost(Name = "CreateBanknote")]
        public IActionResult CreateBanknote([FromBody] BanknoteCreationDto banknote,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (banknote == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var country = _unitOfWork.CountryRepository.GetById(banknote.CountryId);

            if (country != null && banknote.Country == null)
            {
                banknote.Country = country;
            }
            else if (banknote.CountryId != Guid.Empty || 
                banknote.Country.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var collectorValue = _unitOfWork.CollectorValueRepository
                .GetById(banknote.CollectorValueId);

            if (collectorValue != null && banknote.CollectorValue == null)
            {
                banknote.CollectorValue = collectorValue;
            }
            else if (banknote.CollectorValueId != Guid.Empty || 
                banknote.CollectorValue.Id != Guid.Empty)
            {
                return BadRequest();
            }

            var newBanknote = Mapper.Map<Banknote>(banknote);
            _unitOfWork.BanknoteRepository.Add(newBanknote);

            if (!_unitOfWork.Save())
            {
                throw new Exception("Creating a banknote failed on save.");
            }

            var returnedBanknote = Mapper.Map<BanknoteDto>(newBanknote);

            if (mediaType == "application/json+hateoas")
            {
                var links = CreateBanknoteLinks(returnedBanknote.Id, null);
                var linkedResource = returnedBanknote.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResource.Add("links", links);

                return CreatedAtRoute("GetBanknote",
                    new { id = returnedBanknote.Id },
                    linkedResource);
            }
            else
            {
                return CreatedAtRoute("GetBanknote",
                    new { id = returnedBanknote.Id },
                    returnedBanknote);
            }
        }

        [HttpPost("{id}")]
        public IActionResult BlockBanknoteCreation(Guid id)
        {
            if (_unitOfWork.BanknoteRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpPut("{id}", Name = "UpdateBanknote")]
        public IActionResult UpdateBanknote(Guid id, [FromBody] BanknoteUpdateDto banknote)
        {
            if (banknote == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_unitOfWork.CountryRepository.Exists(banknote.CountryId))
            {
                return BadRequest();
            }

            if (!_unitOfWork.CollectorValueRepository.Exists(banknote.CollectorValueId))
            {
                return BadRequest();
            }

            var banknoteFromRepo = _unitOfWork.BanknoteRepository.GetById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            banknoteFromRepo.CountryId = banknote.CountryId;
            banknoteFromRepo.CollectorValueId = banknote.CollectorValueId;

            Mapper.Map(banknote, banknoteFromRepo);
            _unitOfWork.BanknoteRepository.Update(banknoteFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Updating banknote {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBanknote")]
        public IActionResult PartiallyUpdateBanknote(Guid id,
            [FromBody] JsonPatchDocument<BanknoteUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var banknoteFromRepo = _unitOfWork.BanknoteRepository.GetById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var patchedBanknote = Mapper.Map<BanknoteUpdateDto>(banknoteFromRepo);
            patchDoc.ApplyTo(patchedBanknote, ModelState);

            TryValidateModel(patchedBanknote);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_unitOfWork.CountryRepository.Exists(patchedBanknote.CountryId))
            {
                return BadRequest();
            }

            if (!_unitOfWork.CollectorValueRepository.Exists(patchedBanknote.CollectorValueId))
            {
                return BadRequest();
            }

            banknoteFromRepo.CountryId = patchedBanknote.CountryId;
            banknoteFromRepo.CollectorValueId = patchedBanknote.CollectorValueId;

            Mapper.Map(patchedBanknote, banknoteFromRepo);
            _unitOfWork.BanknoteRepository.Update(banknoteFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Patching banknote {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteBanknote")]
        public IActionResult DeleteBanknote(Guid id)
        {
            var banknoteFromRepo = _unitOfWork.BanknoteRepository.GetById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            _unitOfWork.BanknoteRepository.Delete(banknoteFromRepo);

            if (!_unitOfWork.Save())
            {
                throw new Exception($"Deleting banknote {id} failed on save.");
            }

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetBanknotesOptions()
        {
            Response.Headers.Add("Allow", "GET - OPTIONS - POST - PUT - PATCH - DELETE");
            return Ok();
        }

        private string CreateBanknotesResourceUri(CurrenciesResourceParameters resourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _controllerService.UrlHelper.Link("GetBanknotes", new
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
                    return _controllerService.UrlHelper.Link("GetBanknotes", new
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
                    return _controllerService.UrlHelper.Link("GetBanknotes", new
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

        private IEnumerable<LinkDto> CreateBanknoteLinks(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(_controllerService.UrlHelper.Link("GetBanknote",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("CreateBanknote",
                    new { }), "create_banknote", "POST"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("UpdateBanknote",
                    new { id }), "update_banknote", "PUT"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("PartiallyUpdateBanknote",
                    new { id }), "partially_update_banknote", "PATCH"));

                links.Add(new LinkDto(_controllerService.UrlHelper.Link("DeleteBanknote",
                    new { id }), "delete_banknote", "DELETE"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateBanknotesLinks
            (CurrenciesResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateBanknotesResourceUri(resourceParameters,
                ResourceUriType.Current), "self", "GET")
            };

            if (hasNext)
            {
                links.Add(new LinkDto(CreateBanknotesResourceUri(resourceParameters,
                    ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateBanknotesResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}