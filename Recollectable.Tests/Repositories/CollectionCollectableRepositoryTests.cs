using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Specifications.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectionCollectableRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllCollectionCollectables()
        {
            //Act
            var result = await _unitOfWork.CollectionCollectables.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCollectionCollectable()
        {
            //Act
            var result = await _unitOfWork.CollectionCollectables.GetSingle();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_AddsNewCollectionCollectable()
        {
            //Arrange
            Guid id = new Guid("60e55387-ee18-4e5c-866f-7ca1d2d09c0f");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectable newCollectable = new CollectionCollectable
            {
                Id = id,
                CollectionId = collectionId,
                CollectableId = new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48"),
                ConditionId = new Guid("371da3ae-d2e0-4ee7-abf3-3a7574ae669a")
            };

            //Act
            await _unitOfWork.CollectionCollectables.Add(newCollectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.CollectionCollectables.GetAll()).Count());
            Assert.Equal("France", (await _unitOfWork.CollectionCollectables
                .GetSingle(new CollectionCollectableByCollectionId(collectionId) && new CollectionCollectableById(id)))
                .Collectable.Country.Name);
        }

        [Fact]
        public async Task Delete_RemovesCollectableFromDatabase()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectable collectable = await _unitOfWork.CollectionCollectables
                .GetSingle(new CollectionCollectableByCollectionId(collectionId) && new CollectionCollectableById(id));

            //Act
            _unitOfWork.CollectionCollectables.Delete(collectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.CollectionCollectables.GetAll()).Count());
            Assert.Null(await _unitOfWork.CollectionCollectables
                .GetSingle(new CollectionCollectableByCollectionId(collectionId) && new CollectionCollectableById(id)));
        }
    }
}