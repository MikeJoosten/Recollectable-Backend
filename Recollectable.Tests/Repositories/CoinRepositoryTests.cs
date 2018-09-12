using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CoinRepositoryTests : RecollectableTestBase
    {
        private CurrenciesResourceParameters resourceParameters;

        public CoinRepositoryTests()
        {
            resourceParameters = new CurrenciesResourceParameters();
        }

        [Fact]
        public void Get_ReturnsAllCoins()
        {
            var result = _unitOfWork.CoinRepository.Get(resourceParameters);
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersCoinsByCountry()
        {
            var result = _unitOfWork.CoinRepository.Get(resourceParameters);
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Fact]
        public void GetById_ReturnsCoin_GivenValidCoinId()
        {
            var result = _unitOfWork.CoinRepository
                .GetById(new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));
            Assert.NotNull(result);
            Assert.Equal("3a7fd6a5-d654-4647-8374-eba27001b0d3", result.Id.ToString());
            Assert.Equal("Pesos", result.Type);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidCoinId()
        {
            var result = _unitOfWork.CoinRepository
                .GetById(new Guid("4ab72e1b-c115-4f13-b317-a841f73e44b7"));
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewCoin()
        {
            Coin newCoin = new Coin
            {
                Id = new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f"),
                Type = "Cent",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb")
            };

            _unitOfWork.CoinRepository.Add(newCoin);
            _unitOfWork.Save();

            Assert.Equal(7, _unitOfWork.CoinRepository.Get(resourceParameters).Count());
            Assert.Equal("Cent", _unitOfWork.CoinRepository
                .GetById(new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f"))
                .Type);
        }

        [Fact]
        public void Update_UpdatesExistingCoin()
        {
            Coin updatedCoin = _unitOfWork.CoinRepository
                .GetById(new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac"));
            updatedCoin.Type = "Baht";

            _unitOfWork.CoinRepository.Update(updatedCoin);
            _unitOfWork.Save();

            Assert.Equal(6, _unitOfWork.CoinRepository.Get(resourceParameters).Count());
            Assert.Equal("Baht", _unitOfWork.CoinRepository
                .GetById(new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac"))
                .Type);
        }

        [Fact]
        public void Delete_RemovesCoinFromDatabase()
        {
            Coin coin = _unitOfWork.CoinRepository.GetById(new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48"));

            _unitOfWork.CoinRepository.Delete(coin);
            _unitOfWork.Save();

            Assert.Equal(5, _unitOfWork.CoinRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.CoinRepository
                .GetById(new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48")));
        }
    }
}