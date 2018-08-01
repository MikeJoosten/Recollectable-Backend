using Microsoft.EntityFrameworkCore;
using Recollectable.Data;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class UserRepositoryTests : RecollectableTestBase
    {
        private IUserRepository _repository;

        public UserRepositoryTests()
        {
            _repository = new UserRepository(_context);
        }

        [Fact]
        public void GetUsers_ReturnsAllUsers()
        {
            var result = _repository.GetUsers();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetUsers_OrdersUsersByName()
        {
            var result = _repository.GetUsers();
            Assert.Equal("Gavin", result.First().FirstName);
        }

        [Theory]
        [InlineData("4a9522da-66f9-4dfb-88b8-f92b950d1df1", "Ryan")]
        [InlineData("ca26fdfb-46b3-4120-9e52-a07820bc0409", "Jeremy")]
        [InlineData("c7304af2-e5cd-4186-83d9-77807c9512ec", "Michael")]
        public void GetUser_ReturnsUser_GivenValidId(string userId, string expected)
        {
            var result = _repository.GetUser(new Guid(userId));
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id.ToString());
            Assert.Equal(expected, result.FirstName);
        }

        [Theory]
        [InlineData("c798a076-6080-4d40-9b3a-76bf75dc02e9")]
        [InlineData("433c33f0-fa1c-443e-9259-0f24057a7127")]
        [InlineData("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc")]
        public void GetUser_ReturnsNull_GivenInvalidId(string userId)
        {
            var result = _repository.GetUser(new Guid(userId));
            Assert.Null(result);
        }

        [Fact]
        public void AddUser_AddsNewUser()
        {
            User newUser = new User
            {
                Id = new Guid("21ced530-0488-4c40-9543-986c1970e66f"),
                FirstName = "Burnie",
                LastName = "Burns"
            };

            _repository.AddUser(newUser);
            _repository.Save();

            Assert.Equal(7, _repository.GetUsers().Count());
            Assert.Equal("Burnie", _repository
                .GetUser(new Guid("21ced530-0488-4c40-9543-986c1970e66f"))
                .FirstName);
        }

        [Theory]
        [InlineData("4a9522da-66f9-4dfb-88b8-f92b950d1df1", "Alfredo")]
        [InlineData("ca26fdfb-46b3-4120-9e52-a07820bc0409", "Matt")]
        [InlineData("c7304af2-e5cd-4186-83d9-77807c9512ec", "Miles")]
        public void UpdateUser_UpdatesExistingUser(string userId, string updatedName)
        {
            User updatedUser = _repository.GetUser(new Guid(userId));
            updatedUser.FirstName = updatedName;

            _repository.UpdateUser(updatedUser);
            _repository.Save();

            Assert.Equal(6, _repository.GetUsers().Count());
            Assert.Equal(updatedName, _repository
                .GetUser(new Guid(userId))
                .FirstName);
        }

        [Fact]
        public void DeleteUser_RemovesUserFromDatabase()
        {
            User user = _repository.GetUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));

            _repository.DeleteUser(user);
            _repository.Save();

            Assert.Equal(5, _repository.GetUsers().Count());
            Assert.Null(_repository.GetUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")));
        }
    }
}
