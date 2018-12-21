using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class CollectionServiceTests : RecollectableTestBase
    {
        private readonly ICollectionService _collectionService;
        private CollectionsResourceParameters resourceParameters;

        public CollectionServiceTests()
        {
            _collectionService = new CollectionService(_unitOfWork);
            resourceParameters = new CollectionsResourceParameters();
        }

        [Fact]
        public async Task FindCollections_ReturnsAllCollections()
        {
            //Act
            var result = await _collectionService.FindCollections(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task FindCollections_OrdersCollectionsByType()
        {
            //Act
            var result = await _collectionService.FindCollections(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Banknote", result.First().Type);
        }

        [Fact]
        public async Task FindCollectionById_ReturnsCollection_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");

            //Act
            var result = await _collectionService.FindCollectionById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Banknote", result.Type);
        }

        [Fact]
        public async Task FindCollectionById_ReturnsNull_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6");

            //Act
            var result = await _collectionService.FindCollectionById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCollection_CreatesNewCollection()
        {
            //Arrange
            Guid id = new Guid("2cb67024-729e-4d76-bbe4-e80f929557ab");
            Collection newCollection = new Collection
            {
                Id = id,
                Type = "Banknote"
            };

            //Act
            await _collectionService.CreateCollection(newCollection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _collectionService.FindCollections(resourceParameters)).Count());
            Assert.Equal("Banknote", (await _collectionService.FindCollectionById(id)).Type);
        }

        [Fact]
        public async Task UpdateCollection_UpdatesExistingCollection()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");
            Collection updatedCollection = await _collectionService.FindCollectionById(id);
            updatedCollection.Type = "Coin";

            //Act
            _collectionService.UpdateCollection(updatedCollection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _collectionService.FindCollections(resourceParameters)).Count());
            Assert.Equal("Coin", (await _collectionService.FindCollectionById(id)).Type);
        }

        [Fact]
        public async Task RemoveCollection_RemovesCollectionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Collection collection = await _collectionService.FindCollectionById(id);

            //Act
            _collectionService.RemoveCollection(collection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _collectionService.FindCollections(resourceParameters)).Count());
            Assert.Null(await _collectionService.FindCollectionById(id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");

            //Act
            var result = await _collectionService.CollectionExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6");

            //Act
            var result = await _collectionService.CollectionExists(id);

            //Assert
            Assert.False(result);
        }
    }
}