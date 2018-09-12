using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class UserRepositoryTests : RecollectableTestBase
    {
        private UsersResourceParameters resourceParameters;

        public UserRepositoryTests()
        {
            resourceParameters = new UsersResourceParameters();
        }

        [Fact]
        public void Get_ReturnsAllUsers()
        {
            var result = _unitOfWork.UserRepository.Get(resourceParameters);
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersUsersByName()
        {
            var result = _unitOfWork.UserRepository.Get(resourceParameters);
            Assert.Equal("Gavin", result.First().FirstName);
        }

        [Fact]
        public void GetById_ReturnsUser_GivenValidId()
        {
            var result = _unitOfWork.UserRepository
                .GetById(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));
            Assert.NotNull(result);
            Assert.Equal("4a9522da-66f9-4dfb-88b8-f92b950d1df1", result.Id.ToString());
            Assert.Equal("Ryan", result.FirstName);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidId()
        {
            var result = _unitOfWork.UserRepository
                .GetById(new Guid("433c33f0-fa1c-443e-9259-0f24057a7127"));
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewUser()
        {
            User newUser = new User
            {
                Id = new Guid("21ced530-0488-4c40-9543-986c1970e66f"),
                FirstName = "Burnie",
                LastName = "Burns",
                Collections = new List<Collection>()
            };

            _unitOfWork.UserRepository.Add(newUser);
            _unitOfWork.Save();

            Assert.Equal(7, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Equal("Burnie", _unitOfWork.UserRepository
                .GetById(new Guid("21ced530-0488-4c40-9543-986c1970e66f"))
                .FirstName);
        }

        [Fact]
        public void Update_UpdatesExistingUser()
        {
            User updatedUser = _unitOfWork.UserRepository
                .GetById(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));
            updatedUser.FirstName = "Alfredo";

            _unitOfWork.UserRepository.Update(updatedUser);
            _unitOfWork.Save();

            Assert.Equal(6, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Equal("Alfredo", _unitOfWork.UserRepository
                .GetById(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"))
                .FirstName);
        }

        [Fact]
        public void Delete_RemovesUserFromDatabase()
        {
            User user = _unitOfWork.UserRepository
                .GetById(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));

            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();

            Assert.Equal(5, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.UserRepository.GetById(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")));
        }
    }
}