using Recollectable.Data.Repositories;
using Recollectable.Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectorValueRepositoryTests : RecollectableTestBase
    {
        private ICollectorValueRepository _repository;

        /*public CollectorValueRepositoryTests()
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
            Assert.Equal(125.48, result.First().G4);
        }

        [Fact]
        public void GetCollectorValue_ReturnsCollectorValue_GivenValidId()
        {
            var result = _repository
                .GetCollectorValue(new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d"));
            Assert.NotNull(result);
            Assert.Equal("5e9cb33b-b12c-4e20-8113-d8e002aeb38d", result.Id.ToString());
            Assert.Equal(760, result.G4);
        }

        [Fact]
        public void GetCollectorValue_ReturnsNull_GivenInvalidId()
        {
            var result = _repository
                .GetCollectorValue(new Guid("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3"));
            Assert.Null(result);
        }

        [Fact]
        public void AddCollectorValue_AddsNewCollectorValue()
        {
            CollectorValue newCollectorValue = new CollectorValue
            {
                Id = new Guid("3265cf70-f323-4021-932b-08813b1d3d5c"),
                PF60 = 52.15
            };

            _repository.AddCollectorValue(newCollectorValue);
            _repository.Save();

            Assert.Equal(7, _repository.GetCollectorValues().Count());
            Assert.Equal(52.15, _repository
                .GetCollectorValue(new Guid("3265cf70-f323-4021-932b-08813b1d3d5c"))
                .PF60);
        }

        [Fact]
        public void UpdateCollectorValue_UpdatesExistingCollectorValue()
        {
            CollectorValue updatedCollectorValue = 
                _repository.GetCollectorValue(new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb"));
            updatedCollectorValue.G4 = 17.50;

            _repository.UpdateCollectorValue(updatedCollectorValue);
            _repository.Save();

            Assert.Equal(6, _repository.GetCollectorValues().Count());
            Assert.Equal(17.50, _repository
                .GetCollectorValue(new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb"))
                .G4);
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
    }*/
}