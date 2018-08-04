using Microsoft.EntityFrameworkCore;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class BanknoteRepositoryTests : RecollectableTestBase
    {
        private IBanknoteRepository _banknoteRepository;
        private ICountryRepository _countryRepository;
        private ICollectionRepository _collectionRepository;
        private IConditionRepository _conditionRepository;

        public BanknoteRepositoryTests()
        {
            _countryRepository = new CountryRepository(_context);
            _collectionRepository = new CollectionRepository
                (_context, new UserRepository(_context));
            _conditionRepository = new ConditionRepository(_context);
            _banknoteRepository = new BanknoteRepository(_context, _countryRepository,
                _collectionRepository, _conditionRepository);
        }

        [Fact]
        public void GetBanknotes_ReturnsAllBanknotes()
        {
            var result = _banknoteRepository.GetBanknotes();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetBanknotes_OrdersCollectionsByCountry()
        {
            var result = _banknoteRepository.GetBanknotes();
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Theory]
        [InlineData("8c29c8a2-93ae-483d-8235-b0c728d3a034", 1, "Mexico")]
        [InlineData("c8f2031e-c780-4d27-bf13-1ee48a7207a3", 2, "United States of America")]
        public void GetBanknotesByCountry_ReturnsAllBanknotesFromCountry_GivenValidCountryId
            (string countryId, int expectedCount, string expectedName)
        {
            var result = _banknoteRepository
                .GetBanknotesByCountry(new Guid(countryId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedName, result.First().Country.Name);
        }

        [Fact]
        public void GetBanknotesByCountry_OrdersByBanknoteType_GivenValidCountryId()
        {
            var result = _banknoteRepository
                .GetBanknotesByCountry(new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"));
            Assert.Equal("Dollars", result.First().Type);
        }

        [Fact]
        public void GetBanknotesByCountry_ReturnsNull_GivenInvalidCountryId()
        {
            var result = _banknoteRepository
                .GetBanknotesByCountry(new Guid("61a01d02-666c-4d88-b867-ba8d7c134987"));
            Assert.Null(result);
        }

        [Theory]
        [InlineData("6ee10276-5cb7-4c9f-819d-9204274c088a", 1, "Yen")]
        [InlineData("80fa9706-2465-48cf-8933-932fdce18c89", 2, "Dinars")]
        public void GetBanknotesByCollection_ReturnsAllBanknotesFromCollection_GivenValidCollectionId
            (string collectionId, int expectedCount, string expectedType)
        {
            var result = _banknoteRepository
                .GetBanknotesByCollection(new Guid(collectionId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedType, result.First().Type);
        }

        [Fact]
        public void GetBanknotesByCollection_ReturnsNull_GivenInvalidCollectionId()
        {
            var result = _banknoteRepository
                .GetBanknotesByCollection(new Guid("ac4e89d4-6571-42f7-9bc0-482e9700a7ae"));
            Assert.Null(result);
        }

        [Fact]
        public void GetBanknotesByCollection_ReturnsNull_GivenInvalidCollectionType()
        {
            var result = _banknoteRepository
                .GetBanknotesByCollection(new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"));
            Assert.Null(result);
        }

        [Fact]
        public void GetBanknote_ReturnsBanknote_GivenValidBanknoteId()
        {
            var result = _banknoteRepository
                .GetBanknote(new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89"));
            Assert.NotNull(result);
            Assert.Equal("3da0c34f-dbfb-41a3-801f-97b7f4cdde89", result.Id.ToString());
            Assert.Equal("Pounds", result.Type);
        }

        [Fact]
        public void GetBanknote_ReturnsNull_GivenInvalidBanknoteId()
        {
            var result = _collectionRepository
                .GetCollection(new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7"));
            Assert.Null(result);
        }

        [Fact]
        public void AddBanknote_AddsNewBanknote()
        {
            Banknote newBanknote = new Banknote
            {
                Id = new Guid("86dbe5cf-df75-41a5-af56-6e2f2de181a4"),
                Type = "Euros",
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367")
            };

            _banknoteRepository.AddBanknote(newBanknote);
            _banknoteRepository.Save();

            Assert.Equal(7, _banknoteRepository.GetBanknotes().Count());
            Assert.Equal("Euros", _banknoteRepository
                .GetBanknote(new Guid("86dbe5cf-df75-41a5-af56-6e2f2de181a4"))
                .Type);
        }

        [Fact]
        public void AddBanknoteToCollection_AddsNewCollectionCollectable_GivenValidIds()
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("528fc017-4289-492a-b942-bb34a2363d9d"),
                CollectableId = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f"),
                ConditionId = new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b")
            };

            _banknoteRepository.AddBanknoteToCollection(collectionCollectable);
            _banknoteRepository.Save();

            Assert.Single(_banknoteRepository
                .GetBanknotesByCollection(new Guid("528fc017-4289-492a-b942-bb34a2363d9d")));
        }

        [Fact]
        public void AddBanknoteToCollection_AddsNoCollectionCollectable_GivenInvalidCollectionId()
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("a469e351-44c8-4bb6-aec6-114ea1d590fe"),
                CollectableId = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f"),
                ConditionId = new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b")
            };

            _banknoteRepository.AddBanknoteToCollection(collectionCollectable);
            _banknoteRepository.Save();

            Assert.Null(_banknoteRepository
                .GetBanknotesByCollection(new Guid("a469e351-44c8-4bb6-aec6-114ea1d590fe")));
        }

        [Fact]
        public void AddBanknoteToCollection_AddsNoCollectionCollectable_GivenInvalidCollectionType()
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"),
                CollectableId = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f"),
                ConditionId = new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b")
            };

            _banknoteRepository.AddBanknoteToCollection(collectionCollectable);
            _banknoteRepository.Save();

            Assert.Null(_banknoteRepository
                .GetBanknotesByCollection(new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789")));
        }

        [Theory]
        [InlineData("0acf8863-1bec-49a6-b761-ce27dd219e7c", "70d0f180-4a48-4475-a820-0474ba96354a", 2)]
        [InlineData("3a7fd6a5-d654-4647-8374-eba27001b0d3", "0a8d0c2b-1e7f-40b1-980f-eec355e2aca4", 2)]
        [InlineData("65a7dedd-b010-47a9-bf4c-0b62691e71dc", "70d0f180-4a48-4475-a820-0474ba96354a", 2)]
        public void AddBanknoteToCollection_AddsNoCollectionCollectable_GivenInvalidIds
            (string banknoteId, string conditionId, int expectedCount)
        {
            var collectionCollectable = new CollectionCollectable
            {
                CollectionId = new Guid("80fa9706-2465-48cf-8933-932fdce18c89"),
                CollectableId = new Guid(banknoteId),
                ConditionId = new Guid(conditionId)
            };

            _banknoteRepository.AddBanknoteToCollection(collectionCollectable);
            _banknoteRepository.Save();

            Assert.Equal(expectedCount, _banknoteRepository
                .GetBanknotesByCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89"))
                .Count());
        }

        [Fact]
        public void UpdateBanknote_UpdatesExistingBanknote()
        {
            Banknote updatedBanknote = _banknoteRepository
                .GetBanknote(new Guid("48d9049b-04f0-4c24-a1c3-c3668878013e"));
            updatedBanknote.Type = "Euros";

            _banknoteRepository.UpdateBanknote(updatedBanknote);
            _banknoteRepository.Save();

            Assert.Equal(6, _banknoteRepository.GetBanknotes().Count());
            Assert.Equal("Euros", _banknoteRepository
                .GetBanknote(new Guid("48d9049b-04f0-4c24-a1c3-c3668878013e"))
                .Type);
        }

        [Fact]
        public void DeleteBanknote_RemovesBanknoteFromDatabase()
        {
            Banknote banknote = _banknoteRepository
                .GetBanknote(new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c"));

            _banknoteRepository.DeleteBanknote(banknote);
            _banknoteRepository.Save();

            Assert.Equal(5, _banknoteRepository.GetBanknotes().Count());
            Assert.Null(_banknoteRepository
                .GetBanknote(new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c")));
        }

        [Fact]
        public void DeleteBanknoteFromCollection_RemovesCollectionCollectable()
        {
            var banknote = _context.Banknotes
                .Include(b => b.CollectionCollectables)
                .ThenInclude(cc => cc.Collection)
                .SingleOrDefault(b => b.Id == new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4"));

            var collectionCollectable = banknote.CollectionCollectables.SingleOrDefault(cc =>
                cc.CollectionId == new Guid("80fa9706-2465-48cf-8933-932fdce18c89") &&
                cc.ConditionId == new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f"));

            _banknoteRepository.DeleteBanknoteFromCollection(collectionCollectable);
            _banknoteRepository.Save();

            Assert.Single(_banknoteRepository
                .GetBanknotesByCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89")));
        }
    }
}