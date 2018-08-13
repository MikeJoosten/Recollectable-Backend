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
        /*private IUserRepository _repository;

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

        [Fact]
        public void GetUser_ReturnsUser_GivenValidId()
        {
            var result = _repository
                .GetUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));
            Assert.NotNull(result);
            Assert.Equal("4a9522da-66f9-4dfb-88b8-f92b950d1df1", result.Id.ToString());
            Assert.Equal("Ryan", result.FirstName);
        }

        [Fact]
        public void GetUser_ReturnsNull_GivenInvalidId()
        {
            var result = _repository
                .GetUser(new Guid("433c33f0-fa1c-443e-9259-0f24057a7127"));
            Assert.Null(result);
        }

        [Fact]
        public void AddUser_AddsNewUser()
        {
            User newUser = new User
            {
                Id = new Guid("21ced530-0488-4c40-9543-986c1970e66f"),
                FirstName = "Burnie",
                LastName = "Burns",
                Collections = new List<Collection>()
            };

            _repository.AddUser(newUser);
            _repository.Save();

            Assert.Equal(7, _repository.GetUsers().Count());
            Assert.Equal("Burnie", _repository
                .GetUser(new Guid("21ced530-0488-4c40-9543-986c1970e66f"))
                .FirstName);
        }

        [Fact]
        public void UpdateUser_UpdatesExistingUser()
        {
            User updatedUser = _repository
                .GetUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));
            updatedUser.FirstName = "Alfredo";

            _repository.UpdateUser(updatedUser);
            _repository.Save();

            Assert.Equal(6, _repository.GetUsers().Count());
            Assert.Equal("Alfredo", _repository
                .GetUser(new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"))
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
        }*/
    }
}
