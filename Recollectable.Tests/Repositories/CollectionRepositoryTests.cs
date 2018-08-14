using Recollectable.Data.Helpers;
using Recollectable.Data.Repositories;
using Recollectable.Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectionRepositoryTests : RecollectableTestBase
    {
        private ICollectionRepository _collectionRepository;
        private CollectionsResourceParameters resourceParameters;

        public CollectionRepositoryTests()
        {
            _collectionRepository = new CollectionRepository(_context, 
                _propertyMappingService);
            resourceParameters = new CollectionsResourceParameters();
        }

        [Fact]
        public void GetCollections_ReturnsAllCollections()
        {
            var result = _collectionRepository.GetCollections(resourceParameters);
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCollections_OrdersCollectionsByType()
        {
            var result = _collectionRepository.GetCollections(resourceParameters);
            Assert.Equal("Banknote", result.First().Type);
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
        public void UpdateCollection_UpdatesExistingCollection()
        {
            Collection updatedCollection = _collectionRepository
                .GetCollection(new Guid("80fa9706-2465-48cf-8933-932fdce18c89"));
            updatedCollection.Type = "Coin";

            _collectionRepository.UpdateCollection(updatedCollection);
            _collectionRepository.Save();

            Assert.Equal(6, _collectionRepository.GetCollections(resourceParameters).Count());
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

            Assert.Equal(5, _collectionRepository.GetCollections(resourceParameters).Count());
            Assert.Null(_collectionRepository
                .GetCollection(new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12")));
        }
    }
}