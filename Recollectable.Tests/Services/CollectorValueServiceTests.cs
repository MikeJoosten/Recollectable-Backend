using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class CollectorValueServiceTests : RecollectableTestBase
    {
        private readonly ICollectorValueService _collectorValueService;
        private CollectorValuesResourceParameters resourceParameters;

        public CollectorValueServiceTests()
        {
            _collectorValueService = new CollectorValueService(_unitOfWork);
            resourceParameters = new CollectorValuesResourceParameters();
        }

        [Fact]
        public async Task FindCollectorValues_ReturnsAllCollectorValues()
        {
            //Act
            var result = await _collectorValueService.FindCollectorValues(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task FindCollectorValues_OrdersCollectorValueById()
        {
            //Act
            var result = await _collectorValueService.FindCollectorValues(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(125.48, result.First().G4);
        }

        [Fact]
        public async Task FindCollectorValueById_ReturnsCollectorValue_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d");

            //Act
            var result = await _collectorValueService.FindCollectorValueById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(760, result.G4);
        }

        [Fact]
        public async Task FindCollectorValueById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3");

            //Act
            var result = await _collectorValueService.FindCollectorValueById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCollectorValue_CreatesNewCollectorValue()
        {
            //Arrange
            Guid id = new Guid("3265cf70-f323-4021-932b-08813b1d3d5c");
            CollectorValue newCollectorValue = new CollectorValue
            {
                Id = id,
                PF60 = 52.15
            };

            //Act
            await _collectorValueService.CreateCollectorValue(newCollectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _collectorValueService.FindCollectorValues(resourceParameters)).Count());
            Assert.Equal(52.15, (await _collectorValueService.FindCollectorValueById(id)).PF60);
        }

        [Fact]
        public async Task UpdateCollectorValue_UpdatesExistingCollectorValue()
        {
            //Arrange
            Guid id = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb");
            CollectorValue updatedCollectorValue = await _collectorValueService.FindCollectorValueById(id);
            updatedCollectorValue.G4 = 17.50;

            //Act
            _collectorValueService.UpdateCollectorValue(updatedCollectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _collectorValueService.FindCollectorValues(resourceParameters)).Count());
            Assert.Equal(17.50, (await _collectorValueService.FindCollectorValueById(id)).G4);
        }

        [Fact]
        public async Task RemoveCollectorValue_RemovesCollectorValueFromDatabase()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            CollectorValue collectorValue = await _collectorValueService.FindCollectorValueById(id);

            //Act
            _collectorValueService.RemoveCollectorValue(collectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _collectorValueService.FindCollectorValues(resourceParameters)).Count());
            Assert.Null(await _collectorValueService.FindCollectorValueById(id));
        }

        [Fact]
        public async Task CollectorValueExists_ReturnsTrue_GivenValidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d");

            //Act
            var result = await _collectorValueService.CollectorValueExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3");

            //Act
            var result = await _collectorValueService.CollectorValueExists(id);

            //Assert
            Assert.False(result);
        }
    }
}