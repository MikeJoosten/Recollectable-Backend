using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Services;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Collectables;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class CoinsControllerTests : RecollectableTestBase
    {
        private readonly CoinsController _controller;
        private readonly CurrenciesResourceParameters resourceParameters;

        public CoinsControllerTests()
        {
            _controller = new CoinsController(_unitOfWork, _mockControllerService.Object);
            resourceParameters = new CurrenciesResourceParameters();

            SetupTestController<CoinDto, Coin>(_controller,
                PropertyMappingService._currencyPropertyMapping);
        }

        [Fact]
        public void GetCoins_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            _mockPropertyMappingService.Setup(x =>
                x.ValidMappingExistsFor<CoinDto, Coin>(It.IsAny<string>())).Returns(false);

            //Act
            var response = _controller.GetCoins(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }
    }
}