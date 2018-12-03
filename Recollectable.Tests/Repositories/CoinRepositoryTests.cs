using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CoinRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllCoins()
        {
            //Act
            var result = await _unitOfWork.Coins.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCoin()
        {
            //Act
            var result = await _unitOfWork.Coins.GetSingle();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_AddsNewCoin()
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
            await _unitOfWork.Coins.Add(newCoin);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.Coins.GetAll()).Count());
            Assert.Equal("Cent", (await _unitOfWork.Coins.GetSingle(new CoinById(id))).Type);
        }

        [Fact]
        public async Task Delete_RemovesCoinFromDatabase()
        {
            //Arrange
            Guid id = new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48");
            Coin coin = await _unitOfWork.Coins.GetSingle(new CoinById(id));

            //Act
            _unitOfWork.Coins.Delete(coin);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.Coins.GetAll()).Count());
            Assert.Null(await _unitOfWork.Coins.GetSingle(new CoinById(id)));
        }
    }
}