using Microsoft.EntityFrameworkCore;
using Recollectable.Data;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CoinRepositoryTests : RecollectableTestBase
    {
        private ICoinRepository _coinRepository;
        private ICountryRepository _countryRepository;
        private ICollectionRepository _collectionRepository;
        private IConditionRepository _conditionRepository;

        /*public CoinRepositoryTests()
        {
            _countryRepository = new CountryRepository(_context);
            _collectionRepository = new CollectionRepository
                (_context, new UserRepository(_context));
            _conditionRepository = new ConditionRepository(_context);
            _coinRepository = new CoinRepository(_context, _countryRepository, 
                _collectionRepository, _conditionRepository);
        }

        [Fact]
        public void GetCoins_ReturnsAllCoins()
        {
            var result = _coinRepository.GetCoins();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCoins_OrdersCoinsByCountry()
        {
            var result = _coinRepository.GetCoins();
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Theory]
        [InlineData("8cef5964-01a4-40c7-9f16-28af109094d4", 1, "Japan")]
        [InlineData("c8f2031e-c780-4d27-bf13-1ee48a7207a3", 2, "United States of America")]
        public void GetCoinsByCountry_ReturnsAllCoinsFromCountry_GivenValidCountryId
            (string countryId, int expectedCount, string expectedName)
        {
            var result = _coinRepository
                .GetCoinsByCountry(new Guid(countryId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedName, result.First().Country.Name);
        }

        [Fact]
        public void GetCoinsByCountry_OrdersByCoinType_GivenValidCountryId()
        {
            var result = _coinRepository
                .GetCoinsByCountry(new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"));
            Assert.Equal("Dime", result.First().Type);
        }

        [Fact]
        public void GetCoinsByCountry_ReturnsNull_GivenInvalidCountryId()
        {
            var result = _coinRepository
                .GetCoinsByCountry(new Guid("86b7a286-5439-4ec6-8ef3-dca87f790f02"));
            Assert.Null(result);
        }

        [Fact]
        public void GetCoin_ReturnsCoin_GivenValidCoinId()
        {
            var result = _coinRepository
                .GetCoin(new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));
            Assert.NotNull(result);
            Assert.Equal("3a7fd6a5-d654-4647-8374-eba27001b0d3", result.Id.ToString());
            Assert.Equal("Pesos", result.Type);
        }

        [Fact]
        public void GetCoin_ReturnsNull_GivenInvalidCoinId()
        {
            var result = _collectionRepository
                .GetCollection(new Guid("4ab72e1b-c115-4f13-b317-a841f73e44b7"));
            Assert.Null(result);
        }

        [Fact]
        public void AddCoin_AddsNewCoin()
        {
            Coin newCoin = new Coin
            {
                Id = new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f"),
                Type = "Cent",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb")
            };

            _coinRepository.AddCoin(newCoin);
            _coinRepository.Save();

            Assert.Equal(7, _coinRepository.GetCoins().Count());
            Assert.Equal("Cent", _coinRepository
                .GetCoin(new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f"))
                .Type);
        }

        [Fact]
        public void UpdateCoin_UpdatesExistingCoin()
        {
            Coin updatedCoin = _coinRepository
                .GetCoin(new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac"));
            updatedCoin.Type = "Baht";

            _coinRepository.UpdateCoin(updatedCoin);
            _coinRepository.Save();

            Assert.Equal(6, _coinRepository.GetCoins().Count());
            Assert.Equal("Baht", _coinRepository
                .GetCoin(new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac"))
                .Type);
        }

        [Fact]
        public void DeleteCoin_RemovesCoinFromDatabase()
        {
            Coin coin = _coinRepository.GetCoin(new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48"));

            _coinRepository.DeleteCoin(coin);
            _coinRepository.Save();

            Assert.Equal(5, _coinRepository.GetCoins().Count());
            Assert.Null(_coinRepository
                .GetCoin(new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48")));
        }*/
    }
}