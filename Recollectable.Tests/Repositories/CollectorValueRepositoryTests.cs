using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectorValueRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllCollectorValues()
        {
            //Act
            var result = await _unitOfWork.CollectorValues.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCollectorValue()
        {
            //Act
            var result = await _unitOfWork.CollectorValues.GetSingle();

            //Assert
            Assert.NotNull(result);
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
            await _unitOfWork.CollectorValues.Add(newCollectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.CollectorValues.GetAll()).Count());
            Assert.Equal(52.15, (await _unitOfWork.CollectorValues.GetSingle(new CollectorValueById(id))).PF60);
        }

        [Fact]
        public async Task Delete_RemovesCollectorValueFromDatabase()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            CollectorValue collectorValue = await _unitOfWork.CollectorValues.GetSingle(new CollectorValueById(id));

            //Act
            _unitOfWork.CollectorValues.Delete(collectorValue);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.CollectorValues.GetAll()).Count());
            Assert.Null(await _unitOfWork.CollectorValues.GetSingle(new CollectorValueById(id)));
        }
    }
}