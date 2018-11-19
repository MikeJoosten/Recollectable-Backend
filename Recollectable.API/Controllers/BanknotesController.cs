using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recollectable.API.Interfaces;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Enums;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Core.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Controllers
{
    //TODO Add Authorization
    [ApiVersion("1.0")]
    [Route("api/banknotes")]
    public class BanknotesController : Controller
    {
        private IBanknoteRepository _banknoteRepository;
        private ICountryRepository _countryRepository;
        private ICollectorValueRepository _collectorValueRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IMapper _mapper;

        public BanknotesController(IBanknoteRepository banknoteRepository, ICountryRepository countryRepository,
            ICollectorValueRepository collectorValueRepository, ITypeHelperService typeHelperService, 
            IPropertyMappingService propertyMappingService, IMapper mapper)
        {
            _banknoteRepository = banknoteRepository;
            _countryRepository = countryRepository;
            _collectorValueRepository = collectorValueRepository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves banknotes
        /// </summary>
        /// <returns>List of banknotes</returns>
        /// <response code="200">Returns a list of banknotes</response>
        /// <response code="400">Invalid query parameter</response>
        [HttpHead]
        [HttpGet(Name = "GetBanknotes")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(BanknoteDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetBanknotes(CurrenciesResourceParameters resourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<BanknoteDto, Banknote>
                (resourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<BanknoteDto>
                (resourceParameters.Fields))
            {
                return BadRequest();
            }

            var banknotesFromRepo = await _banknoteRepository.GetBanknotes(resourceParameters);
            var banknotes = _mapper.Map<IEnumerable<BanknoteDto>>(banknotesFromRepo);

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

                var linkedCollectionResource = new LinkedCollectionResource
                {
                    Value = linkedBanknotes,
                    Links = links
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

        /// <summary>
        /// Retrieves the requested banknote by banknote ID
        /// </summary>
        /// <param name="id">Banknote ID</param>
        /// <param name="fields">Returned fields</param>
        /// <param name="mediaType"></param>
        /// <returns>Requested banknote</returns>
        /// <response code="200">Returns the requested banknote</response>
        /// <response code="400">Invalid query parameter</response>
        /// <response code="404">Unexisting banknote ID</response>
        [HttpGet("{id}", Name = "GetBanknote")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(BanknoteDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBanknote(Guid id, [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_typeHelperService.TypeHasProperties<BanknoteDto>(fields))
            {
                return BadRequest();
            }

            var banknoteFromRepo = await _banknoteRepository.GetBanknoteById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var banknote = _mapper.Map<BanknoteDto>(banknoteFromRepo);

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

        /// <summary>
        /// Creates a banknote
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /banknotes
        ///     {
        ///         "faceValue": 20,
        ///         "type": "Dollars",
        ///         "releaseDate": "1979",
        ///         "size": "152.4mm x 69.85mm",
        ///         "color": "Deep olive-green on multicolor underprint",
        ///         "signature": "Lawson-Bouey",
        ///         "obverseDescription": "Queen Elizabeth II at right, arms at left",
        ///         "reverseDescription": "Alberta's Lake Moraine and Rocky Mountains",
        ///         "headOfState": "Queen Elizabeth II",
        ///         "countryId": "e8a1c283-2300-4f3f-b408-59d0f8ccd893",
        ///         "collectorValue": {
        ///             "g4": 15.18,
        ///             "vG8": 15.18,
        ///             "f12": 15.18,
        ///             "vF20": 15.18,
        ///             "xF40": 15.18,
        ///             "aU50": 15.18,
        ///             "mS60": 75,
        ///             "mS63": 75
        ///         }
        ///     }
        /// </remarks>
        /// <param name="banknote">Custom banknote</param>
        /// <param name="mediaType"></param>
        /// <returns>Newly created banknote</returns>
        /// <response code="201">Returns the newly created banknote</response>
        /// <response code="400">Invalid banknote</response>
        /// <response code="422">Invalid banknote validation</response>
        [HttpPost(Name = "CreateBanknote")]
        [Consumes("application/json", "application/xml")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(typeof(BanknoteDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateBanknote([FromBody] BanknoteCreationDto banknote,
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

            if (!await _countryRepository.Exists(banknote.CountryId))
            {
                return BadRequest();
            }

            var newBanknote = _mapper.Map<Banknote>(banknote);

            var existingCollectorValue = await _collectorValueRepository.GetCollectorValueByValues(newBanknote.CollectorValue);
            newBanknote.CollectorValueId = existingCollectorValue == null ? Guid.NewGuid() : existingCollectorValue.Id;

            _banknoteRepository.AddBanknote(newBanknote);

            if (!await _banknoteRepository.Save())
            {
                throw new Exception("Creating a banknote failed on save.");
            }

            var returnedBanknote = _mapper.Map<BanknoteDto>(newBanknote);

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

        /// <summary>
        /// Invalid banknote creation request
        /// </summary>
        /// <param name="id">Banknote ID</param>
        /// <response code="404">Unexisting banknote ID</response>
        /// <response code="409">Already existing banknote ID</response>
        [HttpPost("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> BlockBanknoteCreation(Guid id)
        {
            if (await _banknoteRepository.Exists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        /// <summary>
        /// Updates a banknote
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /banknotes/{id}
        ///     {
        ///         "faceValue": 10,
        ///         "type": "Pounds",
        ///         "releaseDate": "1913-1918",
        ///         "size": "171mm x 103mm",
        ///         "color": "Dark blue on multicolor underprint",
        ///         "signature": "J. R. Collins and G. T. Allen",
        ///         "obverseDescription": "Arms at bottom center",
        ///         "reverseDescription": "Horse drawn wagons loaded with sacks of grain (Narwonah, N.S.W.) at center",
        ///         "headOfState": "King George V",
        ///         "countryId": "81b8ec60-9932-44a2-865b-24c8bd1f5916",
        ///         "collectorValue": {
        ///             "g4": 4000,
        ///             "vG8": 4000,
        ///             "f12": 20000,
        ///             "vF20": 20000,
        ///             "xF40": 75000,
        ///             "aU50": 75000,
        ///             "mS60": 75000,
        ///             "mS63": 75000
        ///         }
        ///     }
        /// </remarks>
        /// <param name="id">Banknote ID</param>
        /// <param name="banknote">Custom banknote</param>
        /// <response code="204">Updated the banknote successfully</response>
        /// <response code="400">Invalid banknote</response>
        /// <response code="404">Unexisting banknote ID</response>
        /// <response code="422">Invalid banknote validation</response>
        [HttpPut("{id}", Name = "UpdateBanknote")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateBanknote(Guid id, [FromBody] BanknoteUpdateDto banknote)
        {
            if (banknote == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryRepository.Exists(banknote.CountryId))
            {
                return BadRequest();
            }

            var banknoteFromRepo = await _banknoteRepository.GetBanknoteById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var collectorValue = _mapper.Map<CollectorValue>(banknote.CollectorValue);
            var existingCollectorValue = await _collectorValueRepository.GetCollectorValueByValues(collectorValue);
            banknoteFromRepo.CollectorValueId = existingCollectorValue == null ? Guid.NewGuid() : existingCollectorValue.Id;
            banknoteFromRepo.CollectorValue = collectorValue;

            _mapper.Map(banknote, banknoteFromRepo);
            _banknoteRepository.UpdateBanknote(banknoteFromRepo);

            if (!await _banknoteRepository.Save())
            {
                throw new Exception($"Updating banknote {id} failed on save.");
            }

            return NoContent();
        }

        /// <summary>
        /// Update specific fields of a banknote
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /banknotes/{id}
        ///     [
        ///	        { "op": "replace", "path": "/type", "value": "Pesos" },
        ///	        { "op": "copy", "from": "/signature", "path": "/obverseDescription" },
        ///	        { "op": "move", "from": "/color", "path": "/reverseDescription" },
        ///	        { "op": "remove", "path": "/signature" }
        ///     ]
        /// </remarks>
        /// <param name="id">Banknote ID</param>
        /// <param name="patchDoc">JSON patch document</param>
        [HttpPatch("{id}", Name = "PartiallyUpdateBanknote")]
        public async Task<IActionResult> PartiallyUpdateBanknote(Guid id,
            [FromBody] JsonPatchDocument<BanknoteUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var banknoteFromRepo = await _banknoteRepository.GetBanknoteById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            var patchedBanknote = _mapper.Map<BanknoteUpdateDto>(banknoteFromRepo);
            patchDoc.ApplyTo(patchedBanknote, ModelState);

            TryValidateModel(patchedBanknote);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryRepository.Exists(patchedBanknote.CountryId))
            {
                return BadRequest();
            }

            var collectorValue = _mapper.Map<CollectorValue>(patchedBanknote.CollectorValue);
            var existingCollectorValue = await _collectorValueRepository.GetCollectorValueByValues(collectorValue);
            banknoteFromRepo.CollectorValueId = existingCollectorValue == null ? Guid.NewGuid() : existingCollectorValue.Id;
            banknoteFromRepo.CollectorValue = collectorValue;

            _mapper.Map(patchedBanknote, banknoteFromRepo);
            _banknoteRepository.UpdateBanknote(banknoteFromRepo);

            if (!await _banknoteRepository.Save())
            {
                throw new Exception($"Patching banknote {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteBanknote")]
        public async Task<IActionResult> DeleteBanknote(Guid id)
        {
            var banknoteFromRepo = await _banknoteRepository.GetBanknoteById(id);

            if (banknoteFromRepo == null)
            {
                return NotFound();
            }

            _banknoteRepository.DeleteBanknote(banknoteFromRepo);

            if (!await _banknoteRepository.Save())
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
                    return Url.Link("GetBanknotes", new
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
                    return Url.Link("GetBanknotes", new
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
                    return Url.Link("GetBanknotes", new
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
                links.Add(new LinkDto(Url.Link("GetBanknote",
                    new { id }), "self", "GET"));

                links.Add(new LinkDto(Url.Link("CreateBanknote",
                    new { }), "create_banknote", "POST"));

                links.Add(new LinkDto(Url.Link("UpdateBanknote",
                    new { id }), "update_banknote", "PUT"));

                links.Add(new LinkDto(Url.Link("PartiallyUpdateBanknote",
                    new { id }), "partially_update_banknote", "PATCH"));

                links.Add(new LinkDto(Url.Link("DeleteBanknote",
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