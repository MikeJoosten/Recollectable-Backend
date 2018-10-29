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
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class CoinsControllerTests : RecollectableTestBase
    {
        private readonly CoinsController _controller;
        private readonly CurrenciesResourceParameters resourceParameters;

        public CoinsControllerTests()
        {
            _controller = new CoinsController(_unitOfWork, _typeHelperService,
                _propertyMappingService, _mapper);

            resourceParameters = new CurrenciesResourceParameters();
            SetupTestController<CoinDto, Coin>(_controller);
        }

        [Fact]
        public void GetCoins_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = _controller.GetCoins(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void GetCoins_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = _controller.GetCoins(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public void GetCoins_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = _controller.GetCoins(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetCoins_ReturnsAllCoins_GivenNoMediaType()
        {
            //Act
            var response = _controller.GetCoins(resourceParameters, null) as OkObjectResult;
            var coins = response.Value as List<CoinDto>;

            //Assert
            Assert.NotNull(coins);
            Assert.Equal(6, coins.Count);
        }

        [Fact]
        public void GetCoins_ReturnsAllCoins_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var coins = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(coins);
            Assert.Equal(6, coins.Count);
        }

        [Fact]
        public void GetCoins_ReturnsAllCoins_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public void GetCoins_ReturnsCoins_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var coins = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(coins);
            Assert.Equal(2, coins.Count);
        }

        [Fact]
        public void GetCoins_ReturnsCoins_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = _controller.GetCoins(resourceParameters, mediaType) as OkObjectResult;
            var coins = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(coins);
            Assert.Equal(2, coins.Value.Count());
        }

        [Fact]
        public void GetCoin_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = _controller.GetCoin(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void GetCoin_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("18a1946a-ca7a-4bf4-a642-f5eb846f7dd5");

            //Act
            var response = _controller.GetCoin(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public void GetCoin_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            var response = _controller.GetCoin(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetCoin_ReturnsCoin_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            var response = _controller.GetCoin(id, null, null) as OkObjectResult;
            var coin = response.Value as CoinDto;

            //Assert
            Assert.NotNull(coin);
            Assert.Equal(id, coin.Id);
            Assert.Equal("Dollars", coin.Type);
        }

        [Fact]
        public void GetCoin_ReturnsCoin_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            var response = _controller.GetCoin(id, null, mediaType) as OkObjectResult;
            dynamic coin = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(coin);
            Assert.Equal(id, coin.Id);
            Assert.Equal("Dollars", coin.Type);
        }

        [Fact]
        public void GetCoin_ReturnsCoin_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            var response = _controller.GetCoin(id, null, mediaType) as OkObjectResult;
            dynamic coin = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(coin);
            Assert.Equal(id, coin.Id);
            Assert.Equal("Dollars", coin.Type);
        }

        [Fact]
        public void CreateCoin_ReturnsBadRequestResponse_GivenNoCoin()
        {
            //Act
            var response = _controller.CreateCoin(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void CreateCoin_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = _controller.CreateCoin(coin, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void CreateCoin_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto
            {
                CountryId = new Guid("0fa4202c-c244-4be6-bb47-b8e50aacd7cd")
            };

            //Act
            var response = _controller.CreateCoin(coin, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void CreateCoin_ReturnsBadRequestResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("45672556-e04b-4a14-9572-6f87d0267db5")
            };

            //Act
            var response = _controller.CreateCoin(coin, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public void CreateCoin_ReturnsCreatedResponse_GivenValidCoin(string mediaType)
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.CreateCoin(coin, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public void CreateCoin_CreatesNewCoin_GivenAnyMediaTypeAndValidCoin()
        {
            //Arrange
            CoinCreationDto coin = new CoinCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.CreateCoin(coin, null) as CreatedAtRouteResult;
            var returnedCoin = response.Value as CoinDto;

            //Assert
            Assert.NotNull(returnedCoin);
            Assert.Equal("Dollar", returnedCoin.Type);
            Assert.Equal("United States of America", returnedCoin.Country.Name);
        }

        [Fact]
        public void CreateCoin_CreatesNewCoin_GivenHateoasMediaTypeAndValidCoin()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            CoinCreationDto coin = new CoinCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.CreateCoin(coin, mediaType) as CreatedAtRouteResult;
            dynamic returnedCoin = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCoin);
            Assert.Equal("Dollar", returnedCoin.Type);
            Assert.Equal("United States of America", returnedCoin.Country.Name);
        }

        [Fact]
        public void BlockCoinCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            var response = _controller.BlockCoinCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public void BlockCoinCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("ba5b97b2-e8bd-4953-81ca-f107cb1f5b5d");

            //Act
            var response = _controller.BlockCoinCreation(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void UpdateCoin_ReturnsBadRequestResponse_GivenNoCoin()
        {
            //Act
            var response = _controller.UpdateCoin(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateCoin_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            CoinUpdateDto coin = new CoinUpdateDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = _controller.UpdateCoin(Guid.Empty, coin);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void UpdateCoin_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            CoinUpdateDto coin = new CoinUpdateDto
            {
                CountryId = new Guid("0fa4202c-c244-4be6-bb47-b8e50aacd7cd")
            };

            //Act
            var response = _controller.UpdateCoin(Guid.Empty, coin);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateCoin_ReturnsBadRequestResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            CoinUpdateDto coin = new CoinUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("45672556-e04b-4a14-9572-6f87d0267db5")
            };

            //Act
            var response = _controller.UpdateCoin(Guid.Empty, coin);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateCoin_ReturnsNotFoundResponse_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("46020ac4-f8c6-4bce-8fce-6c8513a49f28");
            CoinUpdateDto coin = new CoinUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.UpdateCoin(id, coin);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void UpdateCoin_ReturnsNoContentResponse_GivenValidCoin()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            CoinUpdateDto coin = new CoinUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.UpdateCoin(id, coin);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void UpdateCoin_UpdatesExistingCoin_GivenValidCoin()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            CoinUpdateDto coin = new CoinUpdateDto
            {
                Type = "Euros",
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.UpdateCoin(id, coin);

            //Assert
            Assert.NotNull(_unitOfWork.CoinRepository.GetById(id));
            Assert.Equal("Euros", _unitOfWork.CoinRepository.GetById(id).Type);
            Assert.Equal("France", _unitOfWork.CoinRepository.GetById(id).Country.Name);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = _controller.PartiallyUpdateCoin(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsNotFoundResponse_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("46020ac4-f8c6-4bce-8fce-6c8513a49f28");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsUnprocessableEntityObjectResponse_GivenEqualSubjectAndNote()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Subject, "Chinese Coin");
            patchDoc.Replace(c => c.Note, "Chinese Coin");

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.CountryId, new Guid("0fa4202c-c244-4be6-bb47-b8e50aacd7cd"));
            patchDoc.Replace(c => c.Subject, "Chinese Coin");

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsBadRequestResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.CollectorValueId, new Guid("45672556-e04b-4a14-9572-6f87d0267db5"));
            patchDoc.Replace(c => c.Subject, "Chinese Coin");

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Subject, "Chinese Coin");

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void PartiallyUpdateCoin_UpdatesExistingCoin_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            JsonPatchDocument<CoinUpdateDto> patchDoc = new JsonPatchDocument<CoinUpdateDto>();
            patchDoc.Replace(c => c.Type, "Euros");
            patchDoc.Replace(c => c.Subject, "Remembrance Coin");
            patchDoc.Replace(c => c.CountryId, new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"));

            //Act
            var response = _controller.PartiallyUpdateCoin(id, patchDoc);

            //Assert
            Assert.NotNull(_unitOfWork.CoinRepository.GetById(id));
            Assert.Equal("Euros", _unitOfWork.CoinRepository.GetById(id).Type);
            Assert.Equal("France", _unitOfWork.CoinRepository.GetById(id).Country.Name);
        }

        [Fact]
        public void DeleteCoin_ReturnsNotFoundResponse_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("46020ac4-f8c6-4bce-8fce-6c8513a49f28");

            //Act
            var response = _controller.DeleteCoin(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void DeleteCoin_ReturnsNoContentResponse_GivenValidCoinId()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            var response = _controller.DeleteCoin(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void DeleteCoin_RemovesCoinFromDatabase()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");

            //Act
            _controller.DeleteCoin(id);

            //Assert
            Assert.Equal(5, _unitOfWork.CoinRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.CoinRepository.GetById(id));
        }
    }
}