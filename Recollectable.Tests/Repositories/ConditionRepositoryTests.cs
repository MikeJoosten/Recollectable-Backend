using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Specifications.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class ConditionRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllConditions()
        {
            //Act
            var result = await _unitOfWork.Conditions.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCondition()
        {
            //Act
            var result = await _unitOfWork.Conditions.GetSingle();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_AddsNewCondition()
        {
            //Arrange
            Guid id = new Guid("14f27e52-1eb3-4d33-a398-72e91ee4a786");
            Condition newCondition = new Condition
            {
                Id = id,
                Grade = "MS64"
            };

            //Act
            await _unitOfWork.Conditions.Add(newCondition);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.Conditions.GetAll()).Count());
            Assert.Equal("MS64", (await _unitOfWork.Conditions.GetSingle(new ConditionById(id))).Grade);
        }

        [Fact]
        public async Task Delete_RemovesConditionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            Condition condition = await _unitOfWork.Conditions.GetSingle(new ConditionById(id));

            //Act
            _unitOfWork.Conditions.Delete(condition);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.Conditions.GetAll()).Count());
            Assert.Null(await _unitOfWork.Conditions.GetSingle(new ConditionById(id)));
        }
    }
}