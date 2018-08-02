using Recollectable.Data;
using Recollectable.Data.Repositories;
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

        public CoinRepositoryTests()
        {
            _countryRepository = new CountryRepository(_context);
            _coinRepository = new CoinRepository(_context, _countryRepository);
        }

        [Fact]
        public void GetCoins_ReturnsAllCoins()
        {
            var result = _coinRepository.GetCoins();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCoins_OrdersCollectionsByCountry()
        {
            var result = _coinRepository.GetCoins();
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Theory]
        [InlineData("8c29c8a2-93ae-483d-8235-b0c728d3a034", 1, "Mexico")]
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

        [Theory]
        [InlineData("6e0940b2-f930-49fb-8b31-7a4c0fc0b403")]
        [InlineData("2135b637-033d-4551-9d93-f729c2bb23e3")]
        [InlineData("40ac0139-3298-4f18-95e6-7a6719bdec07")]
        public void GetCoinsByCountry_ReturnsNull_GivenInvalidCountryId(string countryId)
        {
            var result = _coinRepository
                .GetCoinsByCountry(new Guid(countryId));
            Assert.Null(result);
        }
    }
}