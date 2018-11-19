using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class BanknotesControllerTests : RecollectableTestBase
    {
        private readonly BanknotesController _controller;
        private readonly CurrenciesResourceParameters resourceParameters;

        /*public BanknotesControllerTests()
        {
            _controller = new BanknotesController(_unitOfWork, _typeHelperService, 
                _propertyMappingService, _mapper);

            resourceParameters = new CurrenciesResourceParameters();
            SetupTestController<BanknoteDto, Banknote>(_controller);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetBanknotes_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsAllBanknotes_GivenNoMediaType()
        {
            //Act
            var response = await _controller.GetBanknotes(resourceParameters, null) as OkObjectResult;
            var banknotes = response.Value as List<BanknoteDto>;

            //Assert
            Assert.NotNull(banknotes);
            Assert.Equal(6, banknotes.Count);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsAllBanknotes_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var banknotes = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(banknotes);
            Assert.Equal(6, banknotes.Count);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsAllBanknotes_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public async Task GetBanknotes_ReturnsBanknotes_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var banknotes = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(banknotes);
            Assert.Equal(2, banknotes.Count);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsBanknotes_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var banknotes = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(banknotes);
            Assert.Equal(2, banknotes.Value.Count());
        }

        [Fact]
        public async Task GetBanknote_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetBanknote(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetBanknote_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("64c21a8d-048b-4b2a-8b88-2b2d7919c0be");

            //Act
            var response = await _controller.GetBanknote(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetBanknote_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");

            //Act
            var response = await _controller.GetBanknote(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetBanknote_ReturnsBanknote_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = await _controller.GetBanknote(id, null, null) as OkObjectResult;
            var banknote = response.Value as BanknoteDto;

            //Assert
            Assert.NotNull(banknote);
            Assert.Equal(id, banknote.Id);
            Assert.Equal("Dollars", banknote.Type);
        }

        [Fact]
        public async Task GetBanknote_ReturnsBanknote_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = await _controller.GetBanknote(id, null, mediaType) as OkObjectResult;
            dynamic banknote = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(banknote);
            Assert.Equal(id, banknote.Id);
            Assert.Equal("Dollars", banknote.Type);
        }

        [Fact]
        public async Task GetBanknote_ReturnsBanknote_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = await _controller.GetBanknote(id, null, mediaType) as OkObjectResult;
            dynamic banknote = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(banknote);
            Assert.Equal(id, banknote.Id);
            Assert.Equal("Dollars", banknote.Type);
        }

        [Fact]
        public async Task CreateBanknote_ReturnsBadRequestResponse_GivenNoBanknote()
        {
            //Act
            var response = await _controller.CreateBanknote(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.CreateBanknote(banknote, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task CreateBanknote_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                CountryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d")
            };

            //Act
            var response = await _controller.CreateBanknote(banknote, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateBanknote_ReturnsCreatedResponse_GivenValidBanknote(string mediaType)
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValue = new CollectorValueCreationDto
                {
                    G4 = 124
                }
            };

            //Act
            var response = await _controller.CreateBanknote(banknote, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateBanknote_CreatesNewBanknote_GivenAnyMediaTypeAndValidBanknote()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValue = new CollectorValueCreationDto
                {
                    G4 = 10
                }
            };

            //Act
            var response = await _controller.CreateBanknote(banknote, null) as CreatedAtRouteResult;
            var returnedBanknote = response.Value as BanknoteDto;

            //Assert
            Assert.NotNull(returnedBanknote);
            Assert.Equal("Dollar", returnedBanknote.Type);
            Assert.Equal("United States of America", returnedBanknote.Country.Name);
        }

        [Fact]
        public async Task CreateBanknote_CreatesNewBanknote_GivenHateoasMediaTypeAndValidBanknote()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValue = new CollectorValueCreationDto
                {
                    G4 = 124
                }
            };

            //Act
            var response = await _controller.CreateBanknote(banknote, mediaType) as CreatedAtRouteResult;
            dynamic returnedBanknote = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedBanknote);
            Assert.Equal("Dollar", returnedBanknote.Type);
            Assert.Equal("United States of America", returnedBanknote.Country.Name);
        }

        [Fact]
        public async Task BlockBanknoteCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = await _controller.BlockBanknoteCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BlockBanknoteCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("03c0e206-8222-4c4a-a8e2-4c2e1cae8ad1");

            //Act
            var response = await _controller.BlockBanknoteCreation(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsBadRequestResponse_GivenNoBanknote()
        {
            //Act
            var response = await _controller.UpdateBanknote(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            BanknoteUpdateDto banknote = new BanknoteUpdateDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d")
            };

            //Act
            var response = await _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("1876de77-90d9-4083-91d2-b6a3e6a1bd1c");
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")
            };

            //Act
            var response = await _controller.UpdateBanknote(id, banknote);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsNoContentResponse_GivenValidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValue = new CollectorValueCreationDto
                {
                    G4 = 124
                }
            };

            //Act
            var response = await _controller.UpdateBanknote(id, banknote);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_UpdatesExistingBanknote_GivenValidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                Type = "Euros",
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                CollectorValue = new CollectorValueCreationDto
                {
                    G4 = 124
                }
            };

            //Act
            var response = await _controller.UpdateBanknote(id, banknote);

            //Assert
            Assert.NotNull(await _unitOfWork.BanknoteRepository.GetById(id));
            Assert.Equal("Euros", (await _unitOfWork.BanknoteRepository.GetById(id)).Type);
            Assert.Equal("France", (await _unitOfWork.BanknoteRepository.GetById(id)).Country.Name);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateBanknote(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("1876de77-90d9-4083-91d2-b6a3e6a1bd1c");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(b => b.CountryId, new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d"));

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_UpdatesExistingBanknote_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(b => b.Type, "Euros");
            patchDoc.Replace(b => b.CountryId, new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"));

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.NotNull(await _unitOfWork.BanknoteRepository.GetById(id));
            Assert.Equal("Euros", (await _unitOfWork.BanknoteRepository.GetById(id)).Type);
            Assert.Equal("France", (await _unitOfWork.BanknoteRepository.GetById(id)).Country.Name);
        }

        [Fact]
        public async Task DeleteBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("2d11b36a-9f6b-42f9-a00a-6e07b8a30f0e");

            //Act
            var response = await _controller.DeleteBanknote(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteBanknote_ReturnsNoContentResponse_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = await _controller.DeleteBanknote(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteBanknote_RemovesBanknoteFromDatabase()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            await _controller.DeleteBanknote(id);

            //Assert
            Assert.Equal(5, (await _unitOfWork.BanknoteRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.BanknoteRepository.GetById(id));
        }*/
    }
}