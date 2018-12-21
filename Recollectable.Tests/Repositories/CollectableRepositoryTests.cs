using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectableRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllCollectables()
        {
            //Act
            var result = await _unitOfWork.Collectables.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCollectable()
        {
            //Act
            var result = await _unitOfWork.Collectables.GetSingle();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_AddsNewCollectable()
        {
            //Arrange
            Guid id = new Guid("45640b08-3131-4332-8b62-34c00da71fa1");
            Collectable newCollectable = new Collectable
            {
                Id = id,
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                CollectorValueId = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb")
            };

            //Act
            await _unitOfWork.Collectables.Add(newCollectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(13, (await _unitOfWork.Collectables.GetAll()).Count());
            Assert.Equal("France", (await _unitOfWork.Collectables.GetSingle(new CollectableById(id))).Country.Name);
        }

        [Fact]
        public async Task Delete_RemovesCollectableFromDatabase()
        {
            //Arrange
            Guid id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb");
            Collectable collectable = await _unitOfWork.Collectables.GetSingle(new CollectableById(id));

            //Act
            _unitOfWork.Collectables.Delete(collectable);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(11, (await _unitOfWork.Collectables.GetAll()).Count());
            Assert.Null(await _unitOfWork.Collectables.GetSingle(new CollectableById(id)));
        }
    }
}