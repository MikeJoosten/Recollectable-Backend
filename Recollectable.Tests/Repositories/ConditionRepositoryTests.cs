using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class ConditionRepositoryTests : RecollectableTestBase
    {
        private IConditionRepository _repository;

        public ConditionRepositoryTests()
        {
            _repository = new ConditionRepository(_context);
        }

        /*[Fact]
        public void GetConditions_ReturnsAllConditions()
        {
            var result = _repository.GetConditions();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetConditions_OrdersConditionsByGrade()
        {
            var result = _repository.GetConditions();
            Assert.Equal("AU52", result.First().Grade);
        }

        [Theory]
        [InlineData("80fa9706-2465-48cf-8933-932fdce18c89",
            "28c83ea6-665c-41a0-acb0-92a057228fd4", 1, "Fine")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", 
            "a4b0f559-449f-414c-943e-5e69b6c522fb", 2, "VG10")]
        public void GetConditionsByCollectable_ReturnsAllConditionsFromCollectable
            (string collectionId, string collectableId, int expectedCount, string expectedGrade)
        {
            var result = _repository.GetConditionsByCollectable
                (new Guid(collectionId), new Guid(collectableId));
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count());
            Assert.Equal(expectedGrade, result.First().Grade);
        }

        [Fact]
        public void GetCondition_ReturnsCondition_GivenValidId()
        {
            var result = _repository
                .GetCondition(new Guid("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4"));
            Assert.NotNull(result);
            Assert.Equal("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4", result.Id.ToString());
            Assert.Equal("XF45", result.Grade);
        }

        [Fact]
        public void GetCondition_ReturnsNull_GivenInvalidId()
        {
            var result = _repository
                .GetCondition(new Guid("4e7e331a-f93f-4ae3-a229-7153c17a1ca8"));
            Assert.Null(result);
        }

        [Fact]
        public void AddCondition_AddsNewCondition()
        {
            Condition newCondition = new Condition
            {
                Id = new Guid("ac41b63b-1dfb-4e2f-844f-fca2b43815f8"),
                Grade = "F14"
            };

            _repository.AddCondition(newCondition);
            _repository.Save();

            Assert.Equal(7, _repository.GetConditions().Count());
            Assert.Equal("F14", _repository
                .GetCondition(new Guid("ac41b63b-1dfb-4e2f-844f-fca2b43815f8"))
                .Grade);
        }

        [Fact]
        public void UpdateCondition_UpdatesExistingCondition()
        {
            Condition updatedCondition = _repository
                .GetCondition(new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b"));
            updatedCondition.Grade = "Proof";

            _repository.UpdateCondition(updatedCondition);
            _repository.Save();

            Assert.Equal(6, _repository.GetConditions().Count());
            Assert.Equal("Proof", _repository
                .GetCondition(new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b"))
                .Grade);
        }

        [Fact]
        public void DeleteCondition_RemovesConditionFromDatabase()
        {
            Condition condition = _repository
                .GetCondition(new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f"));

            _repository.DeleteCondition(condition);
            _repository.Save();

            Assert.Equal(5, _repository.GetConditions().Count());
            Assert.Null(_repository
                .GetCondition(new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f")));
        }*/
    }
}