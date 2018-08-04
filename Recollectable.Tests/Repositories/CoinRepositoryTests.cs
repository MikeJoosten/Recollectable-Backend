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

        public CoinRepositoryTests()
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
        public void GetCoins_OrdersCollectionsByCountry()
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

        [Theory]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", 2, "Dollars")]
        [InlineData("ab76b149-09c9-40c8-9b35-e62e53e06c8a", 1, "Yen")]
        public void GetCoinsByCollection_ReturnsAllCoinsFromCollection_GivenValidCollectionId
            (string collectionId, int expectedCount, string expectedType)
        {
            var result = _coinRepository
                .GetCoinsByCollection(new Guid(collectionId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedType, result.First().Type);
        }

        [Fact]
        public void GetCoinsByCollection_ReturnsNull_GivenInvalidCollectionId()
        {
            var result = _coinRepository
                .GetCoinsByCollection(new Guid("8357a057-68d0-4b4b-9d30-4d0fe120499e"));
            Assert.Null(result);
        }

        [Fact]
        public void GetCoinsByCollection_ReturnsNull_GivenInvalidCollectionType()
        {
            var result = _coinRepository
                .GetCoinsByCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89"));
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
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")
            };

            _coinRepository.AddCoin(newCoin);
            _coinRepository.Save();

            Assert.Equal(7, _coinRepository.GetCoins().Count());
            Assert.Equal("Cent", _coinRepository
                .GetCoin(new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f"))
                .Type);
        }

        [Fact]
        public void AddCoinToCollection_AddsNewCollectionCollectable_GivenValidIds()
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12"),
                CollectableId = new Guid("db14f24e-aceb-4315-bfcf-6ace1f9b3613"),
                ConditionId = new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f")
            };

            _coinRepository.AddCoinToCollection(collectionCollectable);
            _coinRepository.Save();

            Assert.Single(_coinRepository
                .GetCoinsByCollection(new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12")));
        }

        [Fact]
        public void AddCoinToCollection_AddsNoCollectionCollectable_GivenInvalidCollectionId()
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("04a7c3bc-d380-4c29-87f2-1276e6cbff0e"),
                CollectableId = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb"),
                ConditionId = new Guid("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4")
            };

            _coinRepository.AddCoinToCollection(collectionCollectable);
            _coinRepository.Save();

            Assert.Null(_coinRepository
                .GetCoinsByCollection(new Guid("04a7c3bc-d380-4c29-87f2-1276e6cbff0e")));
        }

        [Fact]
        public void AddCoinToCollection_AddsNoCollectionCollectable_GivenInvalidCollectionType()
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("80fa9706-2465-48cf-8933-932fdce18c89"),
                CollectableId = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb"),
                ConditionId = new Guid("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4")
            };

            _coinRepository.AddCoinToCollection(collectionCollectable);
            _coinRepository.Save();

            Assert.Null(_coinRepository
                .GetCoinsByCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89")));
        }

        [Theory]
        [InlineData("db14f24e-aceb-4315-bfcf-6ace1f9b3613", "ba3dc023-80e1-4add-a6c0-70ee063b9f64", 2)]
        [InlineData("28c83ea6-665c-41a0-acb0-92a057228fd4", "ef147683-5fa1-48b5-b31f-a95e7264245b", 2)]
        [InlineData("15a9584c-d221-443e-8b77-2a95b4148c3c", "ba3dc023-80e1-4add-a6c0-70ee063b9f64", 2)]
        public void AddCoinToCollection_AddsNoCollectionCollectable_GivenInvalidIds
            (string coinId, string conditionId, int expectedCount)
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"),
                CollectableId = new Guid(coinId),
                ConditionId = new Guid(conditionId)
            };

            _coinRepository.AddCoinToCollection(collectionCollectable);
            _coinRepository.Save();

            Assert.Equal(expectedCount, _coinRepository
                .GetCoinsByCollection(new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"))
                .Count());
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
        }

        [Fact]
        public void DeleteCoinFromCollection_RemovesCollectionCollectable()
        {
            var coin = _context.Coins
                .Include(c => c.CollectionCollectables)
                .ThenInclude(cc => cc.Collection)
                .SingleOrDefault(c => c.Id == new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb"));

            var collectionCollectable = coin.CollectionCollectables.SingleOrDefault(cc => 
                cc.CollectionId == new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789") &&
                cc.ConditionId == new Guid("c2e4d849-c9bf-418d-9269-168a038edcd9"));

            _coinRepository.DeleteCoinFromCollection(collectionCollectable);
            _coinRepository.Save();

            Assert.Single(_coinRepository
                .GetCoinsByCollection(new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789")));
        }
    }
}