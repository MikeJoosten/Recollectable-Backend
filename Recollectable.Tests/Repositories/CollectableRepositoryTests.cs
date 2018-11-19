using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectableRepositoryTests : RecollectableTestBase
    {
        private CollectablesResourceParameters resourceParameters;

        /*public CollectableRepositoryTests()
        {
            resourceParameters = new CollectablesResourceParameters();
        }

        [Fact]
        public async Task Get_ReturnsAllCoins_GivenValidCollectionId()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var result = await _unitOfWork.CollectableRepository
                .Get(collectionId, resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ReturnsNull_GivenInvalidCollectionId()
        {
            //Arrange
            Guid collectionId = new Guid("fa3551fc-3a11-48e8-860f-7457fcaa1fee");

            //Act
            var result = await _unitOfWork.CollectableRepository
                .Get(collectionId, resourceParameters);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetById_ReturnsCollectable_GivenValidIds()
        {
            //Arrange
            Guid id = new Guid("22e513a9-b851-4b93-931c-5904d9120f73");
            Guid collectionId = new Guid("ab76b149-09c9-40c8-9b35-e62e53e06c8a");

            //Act
            var result = await _unitOfWork.CollectableRepository.GetById(collectionId, id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Coin", result.Collection.Type);
        }

        [Theory]
        [InlineData("52c47433-7e1d-464c-b9a8-6832cee99b83", "22e513a9-b851-4b93-931c-5904d9120f73")]
        [InlineData("ab76b149-09c9-40c8-9b35-e62e53e06c8a", "a50266a3-826f-4ce1-b5da-5e6fb6978d63")]
        [InlineData("52c47433-7e1d-464c-b9a8-6832cee99b83", "a50266a3-826f-4ce1-b5da-5e6fb6978d63")]
        public async Task GetById_ReturnsNull_GivenInvalidIds
            (string collectionId, string collectableId)
        {
            //Act
            var result = await _unitOfWork.CollectableRepository.GetById
                (new Guid(collectionId), new Guid(collectableId));

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCollectableItem_ReturnsCollectableItem_GivenValidCollectableItemId()
        {
            //Arrange
            Guid id = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");

            //Act
            var result = await _unitOfWork.CollectableRepository.GetCollectableItem(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Mexico", result.Country.Name);
        }

        [Fact]
        public async Task GetCollectableItem_ReturnsNull_GivenInvalidCollectableItemId()
        {
            //Arrange
            Guid id = new Guid("6ad88c33-4ccf-4850-a4ef-0f84db739a24");

            //Act
            var result = await _unitOfWork.CollectableRepository.GetCollectableItem(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsNewCollectable()
        {
            //Arrange
            Guid id = new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectable newCollectable = new CollectionCollectable
            {
                Id = id,
                CollectionId = collectionId,
                CollectableId = new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48"),
                Condition = "MS62"
            };

            //Act
            _unitOfWork.CollectableRepository.Add(newCollectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(3, (await _unitOfWork.CollectableRepository
                .Get(collectionId, resourceParameters)).Count());
            Assert.Equal("France", (await _unitOfWork.CollectableRepository
                .GetById(collectionId, id)).Collectable.Country.Name);
        }

        [Fact]
        public async Task Update_UpdatesExistingCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectable updatedCollectable = 
                await _unitOfWork.CollectableRepository.GetById(collectionId, id);
            Coin coin = await _unitOfWork.CoinRepository
                .GetById(new Guid("db14f24e-aceb-4315-bfcf-6ace1f9b3613"));
            updatedCollectable.Collectable = coin;

            //Act
            _unitOfWork.CollectableRepository.Update(updatedCollectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(2, (await _unitOfWork.CollectableRepository
                .Get(collectionId, resourceParameters)).Count());
            Assert.Equal("Japan", (await _unitOfWork.CollectableRepository
                .GetById(collectionId, id)).Collectable.Country.Name);
        }

        [Fact]
        public async Task Delete_RemovesCollectableFromDatabase()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectable collectable = 
                await _unitOfWork.CollectableRepository.GetById(collectionId, id);

            //Act
            _unitOfWork.CollectableRepository.Delete(collectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Single(await _unitOfWork.CollectableRepository
                .Get(collectionId, resourceParameters));
            Assert.Null(await _unitOfWork.CollectableRepository
                .GetById(collectionId, id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidIds()
        {
            //Arrange
            Guid id = new Guid("22e513a9-b851-4b93-931c-5904d9120f73");
            Guid collectionId = new Guid("ab76b149-09c9-40c8-9b35-e62e53e06c8a");

            //Act
            var result = await _unitOfWork.CollectableRepository.Exists(collectionId, id);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("52c47433-7e1d-464c-b9a8-6832cee99b83", "22e513a9-b851-4b93-931c-5904d9120f73")]
        [InlineData("ab76b149-09c9-40c8-9b35-e62e53e06c8a", "a50266a3-826f-4ce1-b5da-5e6fb6978d63")]
        [InlineData("52c47433-7e1d-464c-b9a8-6832cee99b83", "a50266a3-826f-4ce1-b5da-5e6fb6978d63")]
        public async Task Exists_ReturnsFalse_GivenInvalidIds(string collectionId, string collectableId)
        {
            //Act
            var result = await _unitOfWork.CollectableRepository
                .Exists(new Guid(collectionId), new Guid(collectableId));

            //Assert
            Assert.False(result);
        }*/
    }
}