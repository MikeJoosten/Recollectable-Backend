using Recollectable.API.Services;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Models.Users;
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

            _mockPropertyMappingService.Setup(x =>
                x.GetPropertyMapping<UserDto, User>())
                .Returns(PropertyMappingService._userPropertyMapping);
        }

        [Fact]
        public void Get_ReturnsAllUsers()
        {
            //Act
            var result = _unitOfWork.UserRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersUsersByName()
        {
            //Act
            var result = _unitOfWork.UserRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Gavin", result.First().FirstName);
        }

        [Fact]
        public void GetById_ReturnsUser_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var result = _unitOfWork.UserRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Ryan", result.FirstName);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("433c33f0-fa1c-443e-9259-0f24057a7127");

            //Act
            var result = _unitOfWork.UserRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewUser()
        {
            //Arrange
            Guid id = new Guid("21ced530-0488-4c40-9543-986c1970e66f");
            User newUser = new User
            {
                Id = id,
                FirstName = "Burnie",
                LastName = "Burns",
                Collections = new List<Collection>()
            };

            //Act
            _unitOfWork.UserRepository.Add(newUser);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(7, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Equal("Burnie", _unitOfWork.UserRepository.GetById(id).FirstName);
        }

        [Fact]
        public void Update_UpdatesExistingUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User updatedUser = _unitOfWork.UserRepository.GetById(id);
            updatedUser.FirstName = "Alfredo";

            //Act
            _unitOfWork.UserRepository.Update(updatedUser);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(6, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Equal("Alfredo", _unitOfWork.UserRepository.GetById(id).FirstName);
        }

        [Fact]
        public void Delete_RemovesUserFromDatabase()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User user = _unitOfWork.UserRepository.GetById(id);

            //Act
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(5, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.UserRepository.GetById(id));
        }

        [Fact]
        public void Exists_ReturnsTrue_GivenValidUserId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var result = _unitOfWork.UserRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_ReturnsFalse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("433c33f0-fa1c-443e-9259-0f24057a7127");

            //Act
            var result = _unitOfWork.UserRepository.Exists(id);

            //Assert
            Assert.False(result);
        }
    }
}