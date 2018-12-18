using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Models.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Tests.Builders;
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
        private readonly Mock<IBanknoteService> _mockBanknoteService;
        private readonly Mock<ICountryService> _mockCountryService;
        private readonly Mock<ICollectorValueService> _mockCollectorValueService;
        private readonly CurrenciesResourceParameters resourceParameters;
        private readonly BanknoteTestBuilder _builder;

        public BanknotesControllerTests()
        {
            _mockBanknoteService = new Mock<IBanknoteService>();
            _mockCountryService = new Mock<ICountryService>();
            _mockCollectorValueService = new Mock<ICollectorValueService>();
            _mockBanknoteService.Setup(b => b.Save()).ReturnsAsync(true);
            _mockCountryService.Setup(c => c.CountryExists(It.IsAny<Guid>())).ReturnsAsync(true);

            _mockCollectorValueService
                .Setup(c => c.FindCollectorValueByValues(It.IsAny<CollectorValue>()))
                .ReturnsAsync(new CollectorValue());

            _controller = new BanknotesController(_mockBanknoteService.Object, 
                _mockCountryService.Object, _mockCollectorValueService.Object, _mapper);
            SetupTestController(_controller);

            _builder = new BanknoteTestBuilder();
            resourceParameters = new CurrenciesResourceParameters();
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
        [InlineData("application/json+hateoas")]
        public async Task GetBanknotes_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            var banknotes = _builder.Build(2);
            var pagedList = PagedList<Banknote>.Create(banknotes, 
                resourceParameters.Page, resourceParameters.PageSize);

            _mockBanknoteService
                .Setup(b => b.FindBanknotes(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsAllBanknotes_GivenAnyMediaType()
        {
            //Arrange
            var banknotes = _builder.Build(2);
            var pagedList = PagedList<Banknote>.Create(banknotes,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockBanknoteService
                .Setup(b => b.FindBanknotes(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsAllBanknotes_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var banknotes = _builder.Build(2);
            var pagedList = PagedList<Banknote>.Create(banknotes,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockBanknoteService
                .Setup(b => b.FindBanknotes(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetBanknotes_ReturnsBanknotes_GivenAnyMediaTypeAndPagingParameters()
        {
            //Arrange
            var banknotes = _builder.Build(4);
            var pagedList = PagedList<Banknote>.Create(banknotes, 1, 2);

            _mockBanknoteService
                .Setup(b => b.FindBanknotes(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetBanknotes_ReturnsBanknotes_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var banknotes = _builder.Build(4);
            var pagedList = PagedList<Banknote>.Create(banknotes, 1, 2);

            _mockBanknoteService
                .Setup(b => b.FindBanknotes(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
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
            //Act
            var response = await _controller.GetBanknote(Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetBanknote_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            var banknote = _builder.Build();

            _mockBanknoteService
                .Setup(b => b.FindBanknoteById(id))
                .ReturnsAsync(banknote);

            //Act
            var response = await _controller.GetBanknote(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetBanknote_ReturnsBanknote_GivenAnyMediaType()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");
            var banknote = _builder.WithId(id).WithType("Dollars").Build();

            _mockBanknoteService
                .Setup(b => b.FindBanknoteById(id))
                .ReturnsAsync(banknote);

            //Act
            var response = await _controller.GetBanknote(id, null, null) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Dollars", result.Type);
        }

        [Fact]
        public async Task GetBanknote_ReturnsBanknote_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");
            var banknote = _builder.WithId(id).WithType("Dollars").Build();

            _mockBanknoteService
                .Setup(b => b.FindBanknoteById(id))
                .ReturnsAsync(banknote);

            //Act
            var response = await _controller.GetBanknote(id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Dollars", result.Type);
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
            var banknote = _builder.BuildCreationDto();
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
            Guid countryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d");
            var banknote = _builder.WithCountryId(countryId).BuildCreationDto();
            _mockCountryService.Setup(c => c.CountryExists(It.IsAny<Guid>())).ReturnsAsync(false);

            //Act
            var response = await _controller.CreateBanknote(banknote, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockCountryService.Verify(c => c.CountryExists(countryId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateBanknote_ReturnsCreatedResponse_GivenValidBanknote(string mediaType)
        {
            //Arrange
            var banknote = _builder.WithType("Dollars").BuildCreationDto();

            //Act
            var response = await _controller.CreateBanknote(banknote, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateBanknote_CreatesNewBanknote_GivenAnyMediaTypeAndValidBanknote()
        {
            //Arrange
            var banknote = _builder.WithType("Dollars").BuildCreationDto();

            //Act
            var response = await _controller.CreateBanknote(banknote, null) as CreatedAtRouteResult;
            var returnedBanknote = response.Value as BanknoteDto;

            //Assert
            Assert.NotNull(returnedBanknote);
            Assert.Equal("Dollars", returnedBanknote.Type);
        }

        [Fact]
        public async Task CreateBanknote_CreatesNewBanknote_GivenHateoasMediaTypeAndValidBanknote()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var banknote = _builder.WithType("Dollars").BuildCreationDto();

            //Act
            var response = await _controller.CreateBanknote(banknote, mediaType) as CreatedAtRouteResult;
            dynamic returnedBanknote = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedBanknote);
            Assert.Equal("Dollars", returnedBanknote.Type);
        }

        [Fact]
        public async Task BlockBanknoteCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");
            _mockBanknoteService.Setup(b => b.BanknoteExists(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            var response = await _controller.BlockBanknoteCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            _mockBanknoteService.Verify(c => c.BanknoteExists(id));
        }

        [Fact]
        public async Task BlockBanknoteCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Act
            var response = await _controller.BlockBanknoteCreation(Guid.Empty);

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
            Guid countryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d");
            var banknote = _builder.WithCountryId(countryId).BuildUpdateDto();
            _mockCountryService.Setup(c => c.CountryExists(It.IsAny<Guid>())).ReturnsAsync(false);

            //Act
            var response = await _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockCountryService.Verify(c => c.CountryExists(countryId));
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            var banknote = _builder.BuildUpdateDto();

            //Act
            var response = await _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateBanknote_ReturnsNoContentResponse_GivenValidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            var banknote = _builder.BuildUpdateDto();
            var retrievedBanknote = _builder.Build();

            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(retrievedBanknote);

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
            var banknote = _builder.BuildUpdateDto();
            var retrievedBanknote = _builder.Build();

            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(retrievedBanknote);
            _mockBanknoteService.Setup(b => b.UpdateBanknote(It.IsAny<Banknote>()));

            //Act
            var response = await _controller.UpdateBanknote(id, banknote);

            //Assert
            _mockBanknoteService.Verify(b => b.UpdateBanknote(retrievedBanknote));
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
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateBanknote(Guid.Empty, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");

            var banknote = _builder.Build();
            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(banknote);

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
            Guid countryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d");

            var banknote = _builder.Build();
            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(banknote);
            _mockCountryService.Setup(c => c.CountryExists(It.IsAny<Guid>())).ReturnsAsync(false);

            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(b => b.CountryId, countryId);

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockCountryService.Verify(c => c.CountryExists(countryId));
        }

        [Fact]
        public async Task PartiallyUpdateBanknote_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            Guid countryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367");

            var banknote = _builder.Build();
            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(banknote);

            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(b => b.Type, "Euros");
            patchDoc.Replace(b => b.CountryId, countryId);

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
            Guid countryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367");

            var banknote = _builder.Build();
            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(banknote);

            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(b => b.Type, "Euros");
            patchDoc.Replace(b => b.CountryId, countryId);

            //Act
            var response = await _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            _mockBanknoteService.Verify(b => b.UpdateBanknote(banknote));
        }

        [Fact]
        public async Task DeleteBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Act
            var response = await _controller.DeleteBanknote(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteBanknote_ReturnsNoContentResponse_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            var banknote = _builder.Build();
            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(banknote);

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

            var banknote = _builder.Build();
            _mockBanknoteService.Setup(b => b.FindBanknoteById(id)).ReturnsAsync(banknote);
            _mockBanknoteService.Setup(b => b.RemoveBanknote(It.IsAny<Banknote>()));

            //Act
            await _controller.DeleteBanknote(id);

            //Assert
            _mockBanknoteService.Verify(b => b.RemoveBanknote(banknote));
        }
    }
}