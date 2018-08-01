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
        [InlineData("c7304af2-e5cd-4186-83d9-77807c9512ec", 2, "Michael")]
        public void GetCollectionsByUser_ReturnsCollectionsOfUser_GivenValidUserId
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

        [Theory]
        [InlineData("2e80bc43-ff19-429a-882a-0d8cacb6bfe3")]
        [InlineData("85e94a88-5166-4b40-8f9a-d514298bec05")]
        [InlineData("6428f1bd-c94c-4a51-ab8c-d85dd7c283a5")]
        public void GetCollectionsByUser_ReturnsNull_GivenInvalidUserId(string userId)
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid(userId));
            Assert.Null(result);
        }

        [Theory]
        [InlineData("03a6907d-4e93-4863-bdaf-1d05140dec12", "Coin")]
        [InlineData("80fa9706-2465-48cf-8933-932fdce18c89", "Banknote")]
        [InlineData("ab76b149-09c9-40c8-9b35-e62e53e06c8a", "Coin")]
        public void GetCollection_ReturnsCollection_GivenValidCollectionId
            (string collectionId, string expectedType)
        {
            var result = _collectionRepository
                .GetCollection(new Guid(collectionId));
            Assert.NotNull(result);
            Assert.Equal(collectionId, result.Id.ToString());
            Assert.Equal(expectedType, result.Type);
        }

        [Theory]
        [InlineData("43a1594e-090f-4ca1-8fcd-f638a96454a3")]
        [InlineData("6ef8fea4-5d71-49ef-954c-910f168a30f1")]
        [InlineData("ca4e2623-304b-49a5-80e4-1f7c7246aac6")]
        public void GetCollection_ReturnsNull_GivenInvalidCollectionId(string collectionId)
        {
            var result = _collectionRepository
                .GetCollection(new Guid(collectionId));
            Assert.Null(result);
        }

        [Theory]
        [InlineData("4a9522da-66f9-4dfb-88b8-f92b950d1df1", 
            "03a6907d-4e93-4863-bdaf-1d05140dec12", "Coin", "Ryan")]
        [InlineData("c7304af2-e5cd-4186-83d9-77807c9512ec", 
            "80fa9706-2465-48cf-8933-932fdce18c89", "Banknote", "Michael")]
        [InlineData("c7304af2-e5cd-4186-83d9-77807c9512ec", 
            "ab76b149-09c9-40c8-9b35-e62e53e06c8a", "Coin", "Michael")]
        public void GetCollectionByUser_ReturnsCollectionOfUser_GivenValidIds
            (string userId, string collectionId, string expectedType, string expectedName)
        {
            var result = _collectionRepository
                .GetCollectionByUser(new Guid(userId), new Guid(collectionId));
            Assert.NotNull(result);
            Assert.Equal(collectionId, result.Id.ToString());
            Assert.Equal(expectedType, result.Type);
            Assert.Equal(expectedName, result.User.FirstName);
        }

        [Theory]
        [InlineData("2e80bc43-ff19-429a-882a-0d8cacb6bfe3", "03a6907d-4e93-4863-bdaf-1d05140dec12")]
        [InlineData("4a9522da-66f9-4dfb-88b8-f92b950d1df1", "ca4e2623-304b-49a5-80e4-1f7c7246aac6")]
        [InlineData("2e80bc43-ff19-429a-882a-0d8cacb6bfe3", "ca4e2623-304b-49a5-80e4-1f7c7246aac6")]
        public void GetCollectionByUser_ReturnsNull_GivenInvalidIds
            (string userId, string collectionId)
        {
            var result = _collectionRepository
                .GetCollectionByUser(new Guid(userId), new Guid(collectionId));
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

        [Theory]
        [InlineData("03a6907d-4e93-4863-bdaf-1d05140dec12", "Banknote")]
        [InlineData("80fa9706-2465-48cf-8933-932fdce18c89", "Coin")]
        [InlineData("ab76b149-09c9-40c8-9b35-e62e53e06c8a", "Banknote")]
        public void UpdateCollection_UpdatesExistingCollection(string collectionId, string updatedType)
        {
            Collection updatedCollection = _collectionRepository
                .GetCollection(new Guid(collectionId));
            updatedCollection.Type = updatedType;

            _collectionRepository.UpdateCollection(updatedCollection);
            _collectionRepository.Save();

            Assert.Equal(6, _collectionRepository.GetCollections().Count());
            Assert.Equal(updatedType, _collectionRepository
                .GetCollection(new Guid(collectionId))
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