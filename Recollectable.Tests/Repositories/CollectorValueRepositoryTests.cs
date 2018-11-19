using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectorValueRepositoryTests : RecollectableTestBase
    {
        private CollectorValuesResourceParameters resourceParameters;

        /*public CollectorValueRepositoryTests()
        {
            resourceParameters = new CollectorValuesResourceParameters();
        }

        [Fact]
        public async Task Get_ReturnsAllCollectorValues()
        {
            //Act
            var result = await _unitOfWork.CollectorValueRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task Get_OrdersCollectorValueById()
        {
            //Act
            var result = await _unitOfWork.CollectorValueRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(125.48, result.First().G4);
        }

        [Fact]
        public async Task GetById_ReturnsCollectorValue_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d");

            //Act
            var result = await _unitOfWork.CollectorValueRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(760, result.G4);
        }

        [Fact]
        public async Task GetById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3");

            //Act
            var result = await _unitOfWork.CollectorValueRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsNewCollectorValue()
        {
            //Arrange
            Guid id = new Guid("3265cf70-f323-4021-932b-08813b1d3d5c");
            CollectorValue newCollectorValue = new CollectorValue
            {
                Id = id,
                PF60 = 52.15
            };

            //Act
            _unitOfWork.CollectorValueRepository.Add(newCollectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.CollectorValueRepository.Get(resourceParameters)).Count());
            Assert.Equal(52.15, (await _unitOfWork.CollectorValueRepository.GetById(id)).PF60);
        }

        [Fact]
        public async Task Update_UpdatesExistingCollectorValue()
        {
            //Arrange
            Guid id = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb");
            CollectorValue updatedCollectorValue = await _unitOfWork.CollectorValueRepository.GetById(id);
            updatedCollectorValue.G4 = 17.50;

            //Act
            _unitOfWork.CollectorValueRepository.Update(updatedCollectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _unitOfWork.CollectorValueRepository.Get(resourceParameters)).Count());
            Assert.Equal(17.50, (await _unitOfWork.CollectorValueRepository.GetById(id)).G4);
        }

        [Fact]
        public async Task Delete_RemovesCollectorValueFromDatabase()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            CollectorValue collectorValue = await _unitOfWork.CollectorValueRepository.GetById(id);

            //Act
            _unitOfWork.CollectorValueRepository.Delete(collectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.CollectorValueRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.CollectorValueRepository.GetById(id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d");

            //Act
            var result = await _unitOfWork.CollectorValueRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3");

            //Act
            var result = await _unitOfWork.CollectorValueRepository.Exists(id);

            //Assert
            Assert.False(result);
        }*/
    }
}