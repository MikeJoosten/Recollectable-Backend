using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectionRepositoryTests : RecollectableTestBase
    {
        private CollectionsResourceParameters resourceParameters;

        public CollectionRepositoryTests()
        {
            resourceParameters = new CollectionsResourceParameters();
        }

        [Fact]
        public async Task Get_ReturnsAllCollections()
        {
            //Act
            var result = await _unitOfWork.CollectionRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task Get_OrdersCollectionsByType()
        {
            //Act
            var result = await _unitOfWork.CollectionRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Banknote", result.First().Type);
        }

        [Fact]
        public async Task GetById_ReturnsCollection_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");

            //Act
            var result = await _unitOfWork.CollectionRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Banknote", result.Type);
        }

        [Fact]
        public async Task GetById_ReturnsNull_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6");

            //Act
            var result = await _unitOfWork.CollectionRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsNewCollection()
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
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.CollectionRepository.Get(resourceParameters)).Count());
            Assert.Equal("Banknote", (await _unitOfWork.CollectionRepository.GetById(id)).Type);
        }

        [Fact]
        public async Task Update_UpdatesExistingCollection()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");
            Collection updatedCollection = await _unitOfWork.CollectionRepository.GetById(id);
            updatedCollection.Type = "Coin";

            //Act
            _unitOfWork.CollectionRepository.Update(updatedCollection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _unitOfWork.CollectionRepository.Get(resourceParameters)).Count());
            Assert.Equal("Coin", (await _unitOfWork.CollectionRepository.GetById(id)).Type);
        }

        [Fact]
        public async Task Delete_RemovesCollectionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Collection collection = await _unitOfWork.CollectionRepository.GetById(id);

            //Act
            _unitOfWork.CollectionRepository.Delete(collection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.CollectionRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.CollectionRepository.GetById(id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89");

            //Act
            var result = await _unitOfWork.CollectionRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("ca4e2623-304b-49a5-80e4-1f7c7246aac6");

            //Act
            var result = await _unitOfWork.CollectionRepository.Exists(id);

            //Assert
            Assert.False(result);
        }
    }
}