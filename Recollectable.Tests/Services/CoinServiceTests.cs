using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class CoinServiceTests : RecollectableTestBase
    {
        private readonly ICoinService _coinService;
        private CurrenciesResourceParameters resourceParameters;

        public CoinServiceTests()
        {
            _coinService = new CoinService(_unitOfWork);
            resourceParameters = new CurrenciesResourceParameters();
        }

        [Fact]
        public async Task FindCoins_ReturnsAllCoins()
        {
            //Act
            var result = await _coinService.FindCoins(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task FindCoins_OrdersCoinsByValue()
        {
            //Act
            var result = await _coinService.FindCoins(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Fact]
        public async Task FindCoinById_ReturnsCoin_GivenValidCoinId()
        {
            //Arrange
            Guid id = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");

            //Act
            var result = await _coinService.FindCoinById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Pesos", result.Type);
        }

        [Fact]
        public async Task FindCoinById_ReturnsNull_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("4ab72e1b-c115-4f13-b317-a841f73e44b7");

            //Act
            var result = await _coinService.FindCoinById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCoin_CreatesNewCoin()
        {
            //Arrange
            Guid id = new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f");
            Coin newCoin = new Coin
            {
                Id = id,
                Type = "Cent",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb")
            };

            //Act
            await _coinService.CreateCoin(newCoin);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _coinService.FindCoins(resourceParameters)).Count());
            Assert.Equal("Cent", (await _coinService.FindCoinById(id)).Type);
        }

        [Fact]
        public async Task UpdateCoin_UpdatesExistingCoin()
        {
            //Arrange
            Guid id = new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac");
            Coin updatedCoin = await _coinService.FindCoinById(id);
            updatedCoin.Type = "Baht";

            //Act
            _coinService.UpdateCoin(updatedCoin);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _coinService.FindCoins(resourceParameters)).Count());
            Assert.Equal("Baht", (await _coinService.FindCoinById(id)).Type);
        }

        [Fact]
        public async Task RemoveCoin_RemovesCoinFromDatabase()
        {
            //Arrange
            Guid id = new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48");
            Coin coin = await _coinService.FindCoinById(id);

            //Act
            _coinService.RemoveCoin(coin);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _coinService.FindCoins(resourceParameters)).Count());
            Assert.Null(await _coinService.FindCoinById(id));
        }

        [Fact]
        public async Task CoinExists_ReturnsTrue_GivenValidCoinId()
        {
            //Arrange
            Guid id = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");

            //Act
            var result = await _coinService.CoinExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CoinExists_ReturnsFalse_GivenInvalidCoinId()
        {
            //Arrange
            Guid id = new Guid("4ab72e1b-c115-4f13-b317-a841f73e44b7");

            //Act
            var result = await _coinService.CoinExists(id);

            //Assert
            Assert.False(result);
        }
    }
}