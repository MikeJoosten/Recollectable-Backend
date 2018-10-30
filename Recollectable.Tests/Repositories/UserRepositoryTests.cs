using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task Get_ReturnsAllUsers()
        {
            //Act
            var result = await _unitOfWork.UserRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task Get_OrdersUsersByName()
        {
            //Act
            var result = await _unitOfWork.UserRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Gavin", result.First().FirstName);
        }

        [Fact]
        public async Task GetById_ReturnsUser_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var result = await _unitOfWork.UserRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Ryan", result.FirstName);
        }

        [Fact]
        public async Task GetById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("433c33f0-fa1c-443e-9259-0f24057a7127");

            //Act
            var result = await _unitOfWork.UserRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsNewUser()
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
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.UserRepository.Get(resourceParameters)).Count());
            Assert.Equal("Burnie", (await _unitOfWork.UserRepository.GetById(id)).FirstName);
        }

        [Fact]
        public async Task Update_UpdatesExistingUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User updatedUser = await _unitOfWork.UserRepository.GetById(id);
            updatedUser.FirstName = "Alfredo";

            //Act
            _unitOfWork.UserRepository.Update(updatedUser);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _unitOfWork.UserRepository.Get(resourceParameters)).Count());
            Assert.Equal("Alfredo", (await _unitOfWork.UserRepository.GetById(id)).FirstName);
        }

        [Fact]
        public async Task Delete_RemovesUserFromDatabase()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User user = await _unitOfWork.UserRepository.GetById(id);

            //Act
            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.UserRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.UserRepository.GetById(id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidUserId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var result = await _unitOfWork.UserRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("433c33f0-fa1c-443e-9259-0f24057a7127");

            //Act
            var result = await _unitOfWork.UserRepository.Exists(id);

            //Assert
            Assert.False(result);
        }
    }
}