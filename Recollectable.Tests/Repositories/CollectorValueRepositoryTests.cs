using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectorValueRepositoryTests : RecollectableTestBase
    {
        private ICollectorValueRepository _repository;

        public CollectorValueRepositoryTests()
        {
            _repository = new CollectorValueRepository(_context);
        }

        [Fact]
        public void GetCollectorValues_ReturnsAllCollectorValues()
        {
            var result = _repository.GetCollectorValues();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCollectorValues_OrdersCollectorValueById()
        {
            var result = _repository.GetCollectorValues();
            Assert.Equal(15.54, result.First().G4Value);
        }

        [Theory]
        [InlineData("843a6427-48ab-421c-ba35-3159b1b024a5", 15.54)]
        [InlineData("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb", 3)]
        [InlineData("5e9cb33b-b12c-4e20-8113-d8e002aeb38d", 760)]
        public void GetCollectorValue_ReturnsCollectorValue_GivenValidId
            (string collectorValueId, double expected)
        {
            var result = _repository.GetCollectorValue(new Guid(collectorValueId));
            Assert.NotNull(result);
            Assert.Equal(collectorValueId, result.Id.ToString());
            Assert.Equal(expected, result.G4Value);
        }

        [Theory]
        [InlineData("526356a1-d0ee-4ddc-9880-4a63bfb696a7")]
        [InlineData("07e7f707-6ae8-463b-a134-488a90c368fa")]
        [InlineData("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3")]
        public void GetCollectorValue_ReturnsNull_GivenInvalidId(string collectorValueId)
        {
            var result = _repository.GetCollectorValue(new Guid(collectorValueId));
            Assert.Null(result);
        }

        [Fact]
        public void AddCollectorValue_CreatesNewCollectorValue()
        {
            CollectorValue newCollectorValue = new CollectorValue
            {
                Id = new Guid("3265cf70-f323-4021-932b-08813b1d3d5c"),
                PF60Value = 52.15
            };

            _repository.AddCollectorValue(newCollectorValue);
            _repository.Save();

            Assert.Equal(7, _repository.GetCollectorValues().Count());
            Assert.Equal(52.15, _repository
                .GetCollectorValue(new Guid("3265cf70-f323-4021-932b-08813b1d3d5c"))
                .PF60Value);
        }

        [Theory]
        [InlineData("843a6427-48ab-421c-ba35-3159b1b024a5", 25.68)]
        [InlineData("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb", 17.50)]
        [InlineData("5e9cb33b-b12c-4e20-8113-d8e002aeb38d", 420)]
        public void UpdateCollectorValue_UpdatesExistingCollectorValue
            (string collectorValueId, double updatedValue)
        {
            CollectorValue updatedCollectorValue = 
                _repository.GetCollectorValue(new Guid(collectorValueId));
            updatedCollectorValue.G4Value = updatedValue;

            _repository.UpdateCollectorValue(updatedCollectorValue);
            _repository.Save();

            Assert.Equal(6, _repository.GetCollectorValues().Count());
            Assert.Equal(updatedValue, _repository
                .GetCollectorValue(new Guid(collectorValueId))
                .G4Value);
        }

        [Fact]
        public void DeleteCollectorValue_RemovesCollectorValueFromDatabase()
        {
            CollectorValue collectorValue = 
                _repository.GetCollectorValue(new Guid("843a6427-48ab-421c-ba35-3159b1b024a5"));

            _repository.DeleteCollectorValue(collectorValue);
            _repository.Save();

            Assert.Equal(5, _repository.GetCollectorValues().Count());
            Assert.Null(_repository
                .GetCollectorValue(new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")));
        }
    }
}