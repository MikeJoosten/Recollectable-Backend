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
    public class CoinsControllerTests : RecollectableTestBase
    {
        private readonly CoinsController _controller;
        private readonly Mock<ICoinService> _mockCoinService;
        private readonly Mock<ICountryService> _mockCountryService;
        private readonly Mock<ICollectorValueService> _mockCollectorValueService;
        private readonly CurrenciesResourceParameters resourceParameters;
        private readonly CoinTestBuilder _builder;

        public CoinsControllerTests()
        {
            _mockCoinService = new Mock<ICoinService>();
            _mockCountryService = new Mock<ICountryService>();
            _mockCollectorValueService = new Mock<ICollectorValueService>();
            _mockCoinService.Setup(c => c.Save()).Returns(Task.FromResult(true));
            _mockCountryService.Setup(c => c.CountryExists(It.IsAny<Guid>())).Returns(Task.FromResult(true));

            _mockCollectorValueService
                .Setup(c => c.FindCollectorValueByValues(It.IsAny<CollectorValue>()))
                .Returns(Task.FromResult(new CollectorValue()));

            _controller = new CoinsController(_mockCoinService.Object,
                _mockCountryService.Object, _mockCollectorValueService.Object, _mapper);

            _builder = new CoinTestBuilder();
            resourceParameters = new CurrenciesResourceParameters();
            SetupTestController(_controller);
        }

        [Fact]
        public async Task GetCoins_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetCoins(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCoins_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetCoins(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCoins_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            var coins = new List<Coin>();
            var pagedList = PagedList<Coin>.Create(coins, resourceParameters.Page, resourceParameters.PageSize);

            _mockCoinService
                .Setup(c => c.FindCoins(resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCoins(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCoins_ReturnsAllCoins_GivenNoMediaType()
        {
            //Arrange
            var coins = _builder.Build(6);
            var pagedList = PagedList<Coin>.Create(coins, resourceParameters.Page, resourceParameters.PageSize);

            _mockCoinService
                .Setup(c => c.FindCoins(resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCoins(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<CoinDto>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public async Task GetCoins_ReturnsAllCoins_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            var coins = _builder.Build(6);
            var pagedList = PagedList<Coin>.Create(coins, resourceParameters.Page, resourceParameters.PageSize);

            _mockCoinService
                .Setup(c => c.FindCoins(resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public async Task GetCoins_ReturnsAllCoins_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var coins = _builder.Build(6);
            var pagedList = PagedList<Coin>.Create(coins, resourceParameters.Page, resourceParameters.PageSize);

            _mockCoinService
                .Setup(c => c.FindCoins(resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Value.Count());
        }

        [Fact]
        public async Task GetCoins_ReturnsCoins_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            var coins = _builder.Build(6);
            var pagedList = PagedList<Coin>.Create(coins, 1, 2);

            _mockCoinService
                .Setup(c => c.FindCoins(resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCoins_ReturnsCoins_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var coins = _builder.Build(6);
            var pagedList = PagedList<Coin>.Create(coins, 1, 2);

            _mockCoinService
                .Setup(c => c.FindCoins(resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCoin_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCoin(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCoin_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Act
            var response = await _controller.GetCoin(Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCoin_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            var coin = _builder.WithId(id).Build();

            _mockCoinService
                .Setup(c => c.FindCoinById(id))
                .Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.GetCoin(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCoin_ReturnsCoin_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            var coin = _builder.WithId(id).WithType("Dollars").Build();

            _mockCoinService
                .Setup(c => c.FindCoinById(id))
                .Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.GetCoin(id, null, null) as OkObjectResult;
            var result = response.Value as CoinDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Dollars", result.Type);
        }

        [Fact]
        public async Task GetCoin_ReturnsCoin_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            var coin = _builder.WithId(id).WithType("Dollars").Build();

            _mockCoinService
                .Setup(c => c.FindCoinById(id))
                .Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.GetCoin(id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Dollars", result.Type);
        }

        [Fact]
        public async Task GetCoin_ReturnsCoin_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            var coin = _builder.WithId(id).WithType("Dollars").Build();

            _mockCoinService
                .Setup(c => c.FindCoinById(id))
                .Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.GetCoin(id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Dollars", result.Type);
        }

        [Fact]
        public async Task CreateCoin_ReturnsBadRequestResponse_GivenNoCoin()
        {
            //Act
            var response = await _controller.CreateCoin(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCoin_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.CreateCoin(coin, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task CreateCoin_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto();

            _mockCountryService
                .Setup(c => c.CountryExists(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            //Act
            var response = await _controller.CreateCoin(coin, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCoin_ReturnsCreatedResponse_GivenValidCoin(string mediaType)
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto
            {
                Type = "Dollar"
            };

            //Act
            var response = await _controller.CreateCoin(coin, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCoin_CreatesNewCoin_GivenAnyMediaTypeAndValidCoin()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto
            {
                Type = "Dollar"
            };

            //Act
            var response = await _controller.CreateCoin(coin, null) as CreatedAtRouteResult;
            var returnedCoin = response.Value as CoinDto;

            //Assert
            Assert.NotNull(returnedCoin);
            Assert.Equal("Dollar", returnedCoin.Type);
        }

        [Fact]
        public async Task CreateCoin_CreatesNewCoin_GivenHateoasMediaTypeAndValidCoin()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            CoinCreationDto coin = new CoinCreationDto
            {
                Type = "Dollar"
            };

            //Act
            var response = await _controller.CreateCoin(coin, mediaType) as CreatedAtRouteResult;
            dynamic returnedCoin = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCoin);
            Assert.Equal("Dollar", returnedCoin.Type);
        }

        [Fact]
        public async Task BlockCoinCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            _mockCoinService
                .Setup(c => c.CoinExists(Guid.Empty))
                .Returns(Task.FromResult(true));

            //Act
            var response = await _controller.BlockCoinCreation(Guid.Empty) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BlockCoinCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Act
            var response = await _controller.BlockCoinCreation(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCoin_ReturnsBadRequestResponse_GivenNoCoin()
        {
            //Act
            var response = await _controller.UpdateCoin(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCoin_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            CoinUpdateDto coin = new CoinUpdateDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.UpdateCoin(Guid.Empty, coin);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCoin_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            CoinUpdateDto coin = new CoinUpdateDto();

            _mockCountryService
                .Setup(c => c.CountryExists(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            //Act
            var response = await _controller.UpdateCoin(Guid.Empty, coin);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCoin_ReturnsNotFoundResponse_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("46020ac4-f8c6-4bce-8fce-6c8513a49f28");
            CoinUpdateDto coin = new CoinUpdateDto();

            //Act
            var response = await _controller.UpdateCoin(id, coin);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCoin_ReturnsNoContentResponse_GivenValidCoin()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            CoinUpdateDto coin = new CoinUpdateDto();

            var retrievedCoin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(retrievedCoin));

            //Act
            var response = await _controller.UpdateCoin(id, coin);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCoin_UpdatesExistingCoin_GivenValidCoin()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            CoinUpdateDto coin = new CoinUpdateDto();

            var retrievedCoin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(retrievedCoin));
            _mockCoinService.Setup(c => c.UpdateCoin(It.IsAny<Coin>()));

            //Act
            var response = await _controller.UpdateCoin(id, coin);

            //Assert
            _mockCoinService.Verify(c => c.UpdateCoin(retrievedCoin));
        }

        [Fact]
        public async Task PartiallyUpdateCoin_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCoin(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCoin_ReturnsNotFoundResponse_GivenInvalidCoinId()
        {
            //Arrange
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCoin(Guid.Empty, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCoin_ReturnsUnprocessableEntityObjectResponse_GivenEqualSubjectAndNote()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Subject, "Chinese Coin");
            patchDoc.Replace(c => c.Note, "Chinese Coin");

            var coin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCoin_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            _controller.ModelState.AddModelError("Type", "Required");

            var coin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCoin_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Subject, "Chinese Coin");

            _mockCountryService
                .Setup(c => c.CountryExists(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            var coin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCoin_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Subject, "Chinese Coin");

            var coin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCoin_UpdatesExistingCoin_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Subject, "Chinese Coin");

            var coin = _builder.Build();
            _mockCoinService.Setup(c => c.FindCoinById(id)).Returns(Task.FromResult(coin));
            _mockCoinService.Setup(c => c.UpdateCoin(It.IsAny<Coin>()));

            //Act
            var response = await _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            _mockCoinService.Verify(b => b.UpdateCoin(coin));
        }

        [Fact]
        public async Task DeleteCoin_ReturnsNotFoundResponse_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("46020ac4-f8c6-4bce-8fce-6c8513a49f28");

            //Act
            var response = await _controller.DeleteCoin(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCoin_ReturnsNoContentResponse_GivenValidCoinId()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            var coin = _builder.Build();
            _mockCoinService.Setup(b => b.FindCoinById(id)).Returns(Task.FromResult(coin));

            //Act
            var response = await _controller.DeleteCoin(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteCoin_RemovesCoinFromDatabase()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            var coin = _builder.Build();
            _mockCoinService.Setup(b => b.FindCoinById(id)).Returns(Task.FromResult(coin));
            _mockCoinService.Setup(b => b.RemoveCoin(It.IsAny<Coin>()));

            //Act
            await _controller.DeleteCoin(id);

            //Assert
            _mockCoinService.Verify(b => b.RemoveCoin(coin));
        }
    }
}