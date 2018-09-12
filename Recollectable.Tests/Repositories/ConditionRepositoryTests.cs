using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class ConditionRepositoryTests : RecollectableTestBase
    {
        private ConditionsResourceParameters resourceParameters;

        public ConditionRepositoryTests()
        {
            resourceParameters = new ConditionsResourceParameters();
        }

        [Fact]
        public void Get_ReturnsAllConditions()
        {
            var result = _unitOfWork.ConditionRepository.Get(resourceParameters);
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersConditionsByGrade()
        {
            var result = _unitOfWork.ConditionRepository.Get(resourceParameters);
            Assert.Equal("AU52", result.First().Grade);
        }

        [Fact]
        public void GetById_ReturnsCondition_GivenValidId()
        {
            var result = _unitOfWork.ConditionRepository
                .GetById(new Guid("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4"));
            Assert.NotNull(result);
            Assert.Equal("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4", result.Id.ToString());
            Assert.Equal("XF45", result.Grade);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidId()
        {
            var result = _unitOfWork.ConditionRepository
                .GetById(new Guid("4e7e331a-f93f-4ae3-a229-7153c17a1ca8"));
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewCondition()
        {
            Condition newCondition = new Condition
            {
                Id = new Guid("ac41b63b-1dfb-4e2f-844f-fca2b43815f8"),
                Grade = "F14"
            };

            _unitOfWork.ConditionRepository.Add(newCondition);
            _unitOfWork.Save();

            Assert.Equal(7, _unitOfWork.ConditionRepository.Get(resourceParameters).Count());
            Assert.Equal("F14", _unitOfWork.ConditionRepository
                .GetById(new Guid("ac41b63b-1dfb-4e2f-844f-fca2b43815f8"))
                .Grade);
        }

        [Fact]
        public void Update_UpdatesExistingCondition()
        {
            Condition updatedCondition = _unitOfWork.ConditionRepository
                .GetById(new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b"));
            updatedCondition.Grade = "Proof";

            _unitOfWork.ConditionRepository.Update(updatedCondition);
            _unitOfWork.Save();

            Assert.Equal(6, _unitOfWork.ConditionRepository.Get(resourceParameters).Count());
            Assert.Equal("Proof", _unitOfWork.ConditionRepository
                .GetById(new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b"))
                .Grade);
        }

        [Fact]
        public void Delete_RemovesConditionFromDatabase()
        {
            Condition condition = _unitOfWork.ConditionRepository
                .GetById(new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f"));

            _unitOfWork.ConditionRepository.Delete(condition);
            _unitOfWork.Save();

            Assert.Equal(5, _unitOfWork.ConditionRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.ConditionRepository
                .GetById(new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f")));
        }
    }
}