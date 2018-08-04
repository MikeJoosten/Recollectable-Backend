using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectionRepositoryTests : RecollectableTestBase
    {
        private IUserRepository _userRepository;
        private ICollectionRepository _collectionRepository;

        public CollectionRepositoryTests()
        {
            _userRepository = new UserRepository(_context);
            _collectionRepository = new CollectionRepository(_context, _userRepository);
        }

        [Fact]
        public void GetCollections_ReturnsAllCollections()
        {
            var result = _collectionRepository.GetCollections();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCollections_OrdersCollectionsByType()
        {
            var result = _collectionRepository.GetCollections();
            Assert.Equal("Banknote", result.First().Type);
        }

        [Theory]
        [InlineData("4a9522da-66f9-4dfb-88b8-f92b950d1df1", 2, "Ryan")]
        [InlineData("e640b01f-9eb8-407f-a8f9-68197a7fe48e", 1, "Geoff")]
        public void GetCollectionsByUser_ReturnsAllCollectionsOfUser_GivenValidUserId
            (string userId, int expectedCount, string expectedName)
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid(userId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedName, result.First().User.FirstName);
        }

        [Fact]
        public void GetCollectionsByUser_OrdersCollectionsByType_GivenValidUserId()
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));
            Assert.Equal("Banknote", result.First().Type);
        }

        [Fact]
        public void GetCollectionsByUser_ReturnsNull_GivenInvalidUserId()
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid("85e94a88-5166-4b40-8f9a-d514298bec05"));
            Assert.Null(result);
        }

        [Fact]
        public void GetCollection_ReturnsCollection_GivenValidCollectionId()
        {
            var result = _collectionRepository
                .GetCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89"));
            Assert.NotNull(result);
            Assert.Equal("80fa9706-2465-48cf-8933-932fdce18c89", result.Id.ToString());
            Assert.Equal("Banknote", result.Type);
        }

        [Fact]
        public void GetCollection_ReturnsNull_GivenInvalidCollectionId()
        {
            var result = _collectionRepository
                .GetCollection(new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6"));
            Assert.Null(result);
        }

        [Fact]
        public void AddCollection_AddsNewCollection_GivenValidUserId()
        {
            Collection newCollection = new Collection
            {
                Id = new Guid("ca64b0d5-d7cc-4efd-93c2-3692317de969"),
                Type = "Banknote"
            };

            _collectionRepository.AddCollection
                (new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), newCollection);
            _collectionRepository.Save();
            Collection actualCollection = _collectionRepository
                .GetCollection(new Guid("ca64b0d5-d7cc-4efd-93c2-3692317de969"));

            Assert.Equal(7, _collectionRepository.GetCollections().Count());
            Assert.Equal("Banknote", actualCollection.Type);
            Assert.Equal("Jack", actualCollection.User.FirstName);
        }

        [Fact]
        public void AddCollection_AddsNoCollection_GivenInvalidUserId()
        {
            Collection newCollection = new Collection
            {
                Id = new Guid("ca64b0d5-d7cc-4efd-93c2-3692317de969"),
                Type = "Banknote"
            };

            _collectionRepository.AddCollection
                (new Guid("6428f1bd-c94c-4a51-ab8c-d85dd7c283a5"), newCollection);
            _collectionRepository.Save();

            Assert.Equal(6, _collectionRepository.GetCollections().Count());
        }

        [Fact]
        public void UpdateCollection_UpdatesExistingCollection()
        {
            Collection updatedCollection = _collectionRepository
                .GetCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89"));
            updatedCollection.Type = "Coin";

            _collectionRepository.UpdateCollection(updatedCollection);
            _collectionRepository.Save();

            Assert.Equal(6, _collectionRepository.GetCollections().Count());
            Assert.Equal("Coin", _collectionRepository
                .GetCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89"))
                .Type);
        }

        [Fact]
        public void DeleteCollection_RemovesCollectionFromDatabase()
        {
            Collection collection = _collectionRepository
                .GetCollection(new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12"));

            _collectionRepository.DeleteCollection(collection);
            _collectionRepository.Save();

            Assert.Equal(5, _collectionRepository.GetCollections().Count());
            Assert.Null(_collectionRepository
                .GetCollection(new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12")));
        }
    }
}