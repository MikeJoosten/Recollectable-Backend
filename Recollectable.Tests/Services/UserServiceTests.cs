using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class UserServiceTests : RecollectableTestBase
    {
        private readonly IUserService _userService;
        private UsersResourceParameters resourceParameters;

        public UserServiceTests()
        {
            _userService = new UserService(_unitOfWork);
            resourceParameters = new UsersResourceParameters();
        }

        [Fact]
        public async Task FindUsers_ReturnsAllUsers()
        {
            //Act
            var result = await _userService.FindUsers(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task FindUsers_OrdersUsersByName()
        {
            //Act
            var result = await _userService.FindUsers(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Gavin", result.First().FirstName);
        }

        [Fact]
        public async Task FindUserById_ReturnsUser_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var result = await _userService.FindUserById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Ryan", result.FirstName);
        }

        [Fact]
        public async Task FindUserById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("433c33f0-fa1c-443e-9259-0f24057a7127");

            //Act
            var result = await _userService.FindUserById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUser_CreatesNewUser()
        {
            //Arrange
            Guid id = new Guid("21ced530-0488-4c40-9543-986c1970e66f");
            User newUser = new User
            {
                Id = id,
                FirstName = "Burnie",
                LastName = "Burns"
            };

            //Act
            await _userService.CreateUser(newUser);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _userService.FindUsers(resourceParameters)).Count());
            Assert.Equal("Burnie", (await _userService.FindUserById(id)).FirstName);
        }

        [Fact]
        public async Task UpdateUser_UpdatesExistingUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User updatedUser = await _userService.FindUserById(id);
            updatedUser.FirstName = "Alfredo";

            //Act
            _userService.UpdateUser(updatedUser);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _userService.FindUsers(resourceParameters)).Count());
            Assert.Equal("Alfredo", (await _userService.FindUserById(id)).FirstName);
        }

        [Fact]
        public async Task RemoveUser_RemovesUserFromDatabase()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User user = await _userService.FindUserById(id);

            //Act
            _userService.RemoveUser(user);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _userService.FindUsers(resourceParameters)).Count());
            Assert.Null(await _userService.FindUserById(id));
        }

        [Fact]
        public async Task UserExists_ReturnsTrue_GivenValidUserId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var result = await _userService.UserExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExists_ReturnsFalse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("433c33f0-fa1c-443e-9259-0f24057a7127");

            //Act
            var result = await _userService.UserExists(id);

            //Assert
            Assert.False(result);
        }
    }
}