using Recollectable.Core.Entities.Users;
using Recollectable.Core.Specifications.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class UserRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            //Act
            var result = await _unitOfWork.Users.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsUser()
        {
            //Act
            var result = await _unitOfWork.Users.GetSingle();

            //Assert
            Assert.NotNull(result);
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
                LastName = "Burns"
            };

            //Act
            await _unitOfWork.Users.Add(newUser);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.Users.GetAll()).Count());
            Assert.Equal("Burnie", (await _unitOfWork.Users.GetSingle(new UserById(id))).FirstName);
        }

        [Fact]
        public async Task Delete_RemovesUserFromDatabase()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            User user = await _unitOfWork.Users.GetSingle(new UserById(id));

            //Act
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.Users.GetAll()).Count());
            Assert.Null(await _unitOfWork.Users.GetSingle(new UserById(id)));
        }
    }
}