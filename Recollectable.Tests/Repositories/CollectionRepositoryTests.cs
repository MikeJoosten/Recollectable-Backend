using Recollectable.Data.Repositories;
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
        public void GetCollectionsByUser_ReturnsCollectionsOfUser_GivenValidId
            (string userId, int expectedCount, string expectedName)
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid(userId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedName, result.First().User.FirstName);
        }

        [Fact]
        public void GetCollectionByUser_OrdersCollectionsByType_GivenValidId()
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));
            Assert.Equal("Banknote", result.First().Type);
        }

        [Theory]
        [InlineData("2e80bc43-ff19-429a-882a-0d8cacb6bfe3")]
        public void GetCollectionsByUser_ReturnsNull_GivenInvalidId(string userId)
        {
            var result = _collectionRepository
                .GetCollectionsByUser(new Guid(userId));
            Assert.Null(result);
        }
    }
}