using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class ConditionServiceTests : RecollectableTestBase
    {
        private readonly IConditionService _conditionService;
        private ConditionsResourceParameters resourceParameters;

        public ConditionServiceTests()
        {
            _conditionService = new ConditionService(_unitOfWork);
            resourceParameters = new ConditionsResourceParameters();
        }

        [Fact]
        public async Task FindConditions_ReturnsAllConditions()
        {
            //Act
            var result = await _conditionService.FindConditions(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task FindConditions_OrdersConditionsByGrade()
        {
            //Act
            var result = await _conditionService.FindConditions(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("AU52", result.First().Grade);
        }

        [Fact]
        public async Task FindConditionById_ReturnsCondition_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("e55b0420-f390-41e6-9100-212b611bbca7");

            //Act
            var result = await _conditionService.FindConditionById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("XF45", result.Grade);
        }

        [Fact]
        public async Task FindConditionById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("642a204c-4b1d-4c63-aa3b-6ef6480685eb");

            //Act
            var result = await _conditionService.FindConditionById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCondition_CreatesNewCondition()
        {
            //Arrange
            Guid id = new Guid("a818b39c-94a0-451a-bd4d-74c65204f79e");
            Condition newCondition = new Condition
            {
                Id = id,
                Grade = "MS64",
                LanguageCode = "en-US"
            };

            //Act
            await _conditionService.CreateCondition(newCondition);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _conditionService.FindConditions(resourceParameters)).Count());
            Assert.Equal("MS64", (await _conditionService.FindConditionById(id)).Grade);
        }

        [Fact]
        public async Task UpdateCondition_UpdatesExistingCondition()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            Condition updatedCondition = await _conditionService.FindConditionById(id);
            updatedCondition.Grade = "F16";

            //Act
            _conditionService.UpdateCondition(updatedCondition);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(4, (await _conditionService.FindConditions(resourceParameters)).Count());
            Assert.Equal("F16", (await _conditionService.FindConditionById(id)).Grade);
        }

        [Fact]
        public async Task RemoveCondition_RemovesConditionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("58f7b2c7-b8fc-48dc-83ab-862a85c80fc8");
            Condition condition = await _conditionService.FindConditionById(id);

            //Act
            _conditionService.RemoveCondition(condition);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(3, (await _conditionService.FindConditions(resourceParameters)).Count());
            Assert.Null(await _conditionService.FindConditionById(id));
        }

        [Fact]
        public async Task ConditionExists_ReturnsTrue_GivenValidConditionId()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");

            //Act
            var result = await _conditionService.ConditionExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ConditionExists_ReturnsFalse_GivenInvalidConditionId()
        {
            //Arrange
            Guid id = new Guid("12aac809-3a87-4345-ac7b-cc7e2f3f4f55");

            //Act
            var result = await _conditionService.ConditionExists(id);

            //Assert
            Assert.False(result);
        }
    }
}