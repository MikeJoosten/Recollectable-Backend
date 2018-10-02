using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
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
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            var result = _unitOfWork.UserRepository.GetById(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
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
            Guid id = new Guid("21ced530-0488-4c40-9543-986c1970e66f");
            User newUser = new User
            {
                Id = id,
                FirstName = "Burnie",
                LastName = "Burns",
                Collections = new List<Collection>()
            };

            _unitOfWork.UserRepository.Add(newUser);
            _unitOfWork.Save();

            Assert.Equal(7, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Equal("Burnie", _unitOfWork.UserRepository.GetById(id).FirstName);
        }

        [Fact]
        public void Update_UpdatesExistingUser()
        {
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User updatedUser = _unitOfWork.UserRepository.GetById(id);
            updatedUser.FirstName = "Alfredo";

            _unitOfWork.UserRepository.Update(updatedUser);
            _unitOfWork.Save();

            Assert.Equal(6, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Equal("Alfredo", _unitOfWork.UserRepository.GetById(id).FirstName);
        }

        [Fact]
        public void Delete_RemovesUserFromDatabase()
        {
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User user = _unitOfWork.UserRepository.GetById(id);

            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();

            Assert.Equal(5, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.UserRepository.GetById(id));
        }

        [Fact]
        public void Exists_ReturnsTrue_GivenValidUserId()
        {
            var result = _unitOfWork.UserRepository
                .Exists(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));

            Assert.True(result);
        }

        [Fact]
        public void Exists_ReturnsFalse_GivenInvalidUserId()
        {
            var result = _unitOfWork.UserRepository
                .Exists(new Guid("433c33f0-fa1c-443e-9259-0f24057a7127"));

            Assert.False(result);
        }
    }
}