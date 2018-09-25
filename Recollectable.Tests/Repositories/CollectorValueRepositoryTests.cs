﻿using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectorValueRepositoryTests : RecollectableTestBase
    {
        private CollectorValuesResourceParameters resourceParameters;

        public CollectorValueRepositoryTests()
        {
            resourceParameters = new CollectorValuesResourceParameters();
        }

        [Fact]
        public void Get_ReturnsAllCollectorValues()
        {
            var result = _unitOfWork.CollectorValueRepository.Get(resourceParameters);
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersCollectorValueById()
        {
            var result = _unitOfWork.CollectorValueRepository.Get(resourceParameters);
            Assert.Equal(125.48, result.First().G4);
        }

        [Fact]
        public void GetById_ReturnsCollectorValue_GivenValidId()
        {
            var result = _unitOfWork.CollectorValueRepository
                .GetById(new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d"));
            Assert.NotNull(result);
            Assert.Equal("5e9cb33b-b12c-4e20-8113-d8e002aeb38d", result.Id.ToString());
            Assert.Equal(760, result.G4);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidId()
        {
            var result = _unitOfWork.CollectorValueRepository
                .GetById(new Guid("ea80ff8d-c263-4b0e-8dea-0e9d31c751f3"));
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewCollectorValue()
        {
            CollectorValue newCollectorValue = new CollectorValue
            {
                Id = new Guid("3265cf70-f323-4021-932b-08813b1d3d5c"),
                PF60 = 52.15
            };

            _unitOfWork.CollectorValueRepository.Add(newCollectorValue);
            _unitOfWork.Save();

            Assert.Equal(7, _unitOfWork.CollectorValueRepository.Get(resourceParameters).Count());
            Assert.Equal(52.15, _unitOfWork.CollectorValueRepository
                .GetById(new Guid("3265cf70-f323-4021-932b-08813b1d3d5c"))
                .PF60);
        }

        [Fact]
        public void Update_UpdatesExistingCollectorValue()
        {
            CollectorValue updatedCollectorValue = 
                _unitOfWork.CollectorValueRepository.GetById(new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb"));
            updatedCollectorValue.G4 = 17.50;

            _unitOfWork.CollectorValueRepository.Update(updatedCollectorValue);
            _unitOfWork.Save();

            Assert.Equal(6, _unitOfWork.CollectorValueRepository.Get(resourceParameters).Count());
            Assert.Equal(17.50, _unitOfWork.CollectorValueRepository
                .GetById(new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb"))
                .G4);
        }

        [Fact]
        public void Delete_RemovesCollectorValueFromDatabase()
        {
            CollectorValue collectorValue = 
                _unitOfWork.CollectorValueRepository.GetById(new Guid("843a6427-48ab-421c-ba35-3159b1b024a5"));

            _unitOfWork.CollectorValueRepository.Delete(collectorValue);
            _unitOfWork.Save();

            Assert.Equal(5, _unitOfWork.CollectorValueRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.CollectorValueRepository
                .GetById(new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")));
        }
    }
}