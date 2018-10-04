using Recollectable.API.Services;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Collections;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectionRepositoryTests : RecollectableTestBase
    {
        private CollectionsResourceParameters resourceParameters;

        public CollectionRepositoryTests()
        {
            resourceParameters = new CollectionsResourceParameters();

            _mockPropertyMappingService.Setup(x =>
                x.GetPropertyMapping<CollectionDto, Collection>())
                .Returns(PropertyMappingService._collectionPropertyMapping);
        }

        [Fact]
        public void Get_ReturnsAllCollections()
        {
            //Act
            var result = _unitOfWork.CollectionRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersCollectionsByType()
        {
            //Act
            var result = _unitOfWork.CollectionRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Banknote", result.First().Type);
        }

        [Fact]
        public void GetById_ReturnsCollection_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");

            //Act
            var result = _unitOfWork.CollectionRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Banknote", result.Type);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6");

            //Act
            var result = _unitOfWork.CollectionRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewCollection()
        {
            //Arrange
            Guid id = new Guid("2cb67024-729e-4d76-bbe4-e80f929557ab");
            Collection newCollection = new Collection
            {
                Id = id,
                Type = "Banknote"
            };

            //Act
            _unitOfWork.CollectionRepository.Add(newCollection);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(7, _unitOfWork.CollectionRepository.Get(resourceParameters).Count());
            Assert.Equal("Banknote", _unitOfWork.CollectionRepository.GetById(id).Type);
        }

        [Fact]
        public void Update_UpdatesExistingCollection()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");
            Collection updatedCollection = _unitOfWork.CollectionRepository.GetById(id);
            updatedCollection.Type = "Coin";

            //Act
            _unitOfWork.CollectionRepository.Update(updatedCollection);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(6, _unitOfWork.CollectionRepository.Get(resourceParameters).Count());
            Assert.Equal("Coin", _unitOfWork.CollectionRepository.GetById(id).Type);
        }

        [Fact]
        public void Delete_RemovesCollectionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Collection collection = _unitOfWork.CollectionRepository.GetById(id);

            //Act
            _unitOfWork.CollectionRepository.Delete(collection);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(5, _unitOfWork.CollectionRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.CollectionRepository.GetById(id));
        }

        [Fact]
        public void Exists_ReturnsTrue_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");

            //Act
            var result = _unitOfWork.CollectionRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_ReturnsFalse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6");

            //Act
            var result = _unitOfWork.CollectionRepository.Exists(id);

            //Assert
            Assert.False(result);
        }
    }
}