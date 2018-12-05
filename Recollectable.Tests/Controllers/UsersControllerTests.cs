using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Models.Users;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class UsersControllerTests : RecollectableTestBase
    {
        private readonly UsersController _controller;
        private readonly UsersResourceParameters resourceParameters;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<IUserService> _mockUserService;
        private Mock<IUserStore<User>> _mockUserStore;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ITokenFactory> _mockTokenFactory;

        public UsersControllerTests()
        {
            _mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>
                (_mockUserStore.Object, null, null, null, null, null, null, null, null);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockEmailService = new Mock<IEmailService>();
            _mockTokenFactory = new Mock<ITokenFactory>();

            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(c => c.Save()).Returns(Task.FromResult(true));

            _controller = new UsersController(_mockUserService.Object, _mockUserManager.Object, 
                _mockTokenFactory.Object, _mockEmailService.Object, _mapper);

            resourceParameters = new UsersResourceParameters();
            SetupTestController<UserDto, User>(_controller);
        }

        /*[Fact]
        public async Task GetUsers_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetUsers(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetUsers_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetUsers(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetUsers_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers_GivenNoMediaType()
        {
            //Act
            var response = await _controller.GetUsers(resourceParameters, null) as OkObjectResult;
            var users = response.Value as List<UserDto>;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(6, users.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var users = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(6, users.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public async Task GetUsers_ReturnsUsers_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var users = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsUsers_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var users = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Value.Count());
        }

        [Fact]
        public async Task GetUser_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetUser(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("72e2cde4-0aec-47e7-9549-f2c578b2c21c");

            //Act
            var response = await _controller.GetUser(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetUser_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = await _controller.GetUser(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = await _controller.GetUser(id, null, null) as OkObjectResult;
            var user = response.Value as UserDto;

            //Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal("Ryan Haywood", user.Name);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = await _controller.GetUser(id, null, mediaType) as OkObjectResult;
            dynamic user = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal("Ryan Haywood", user.Name);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = await _controller.GetUser(id, null, mediaType) as OkObjectResult;
            dynamic user = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal("Ryan Haywood", user.Name);
        }

        [Fact]
        public async Task Register_ReturnsBadRequestResponse_GivenNoUser()
        {
            //Act
            var response = await _controller.Register(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task Register_ReturnsUnprocessableEntityObjectResponse_GivenInvalidUser()
        {
            //Arrange
            UserCreationDto user = new UserCreationDto();
            _controller.ModelState.AddModelError("FirstName", "Required");

            //Act
            var response = await _controller.Register(user, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task Register_ReturnsUnprocessableEntityObjectResponse_GivenInvalidPassword()
        {
            //Arrange
            UserCreationDto user = new UserCreationDto();
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "invalid password" }));

            //Act
            var response = await _controller.Register(user, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task Register_ReturnsUnprocessableEntityObjectResponse_GivenInvalidRole()
        {
            //Arrange
            UserCreationDto user = new UserCreationDto();
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "invalid role" }));

            //Act
            var response = await _controller.Register(user, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task Register_ReturnsCreatedResponse_GivenValidUser(string mediaType)
        {
            //Arrange
            UserCreationDto user = new UserCreationDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = await _controller.Register(user, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task Register_CreatesNewUser_GivenAnyMediaTypeAndValidUser()
        {
            //Arrange
            UserCreationDto user = new UserCreationDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = await _controller.Register(user, null) as CreatedAtRouteResult;
            var returnedUser = response.Value as UserDto;

            //Assert
            Assert.NotNull(returnedUser);
            Assert.Equal("Kara Eberle", returnedUser.Name);
        }

        [Fact]
        public async Task Register_CreatesNewUser_GivenHateoasMediaTypeAndValidUser()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            UserCreationDto user = new UserCreationDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = await _controller.Register(user, mediaType) as CreatedAtRouteResult;
            dynamic returnedUser = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedUser);
            Assert.Equal("Kara Eberle", returnedUser.Name);
        }

        [Fact]
        public async Task Login_ReturnsBadRequestResponse_GivenNoCredentials()
        {
            //Act
            var response = await _controller.Login(null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task Login_ReturnsNotFoundResponse_GivenInvalidUser()
        {
            //Arrange
            CredentialsDto credentials = new CredentialsDto
            {
                UserName = "Invalid"
            };

            //Act
            var response = await _controller.Login(credentials);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsNotFoundResponse_GivenInvalidEmail()
        {
            //Arrange
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            //Act
            var response = await _controller.ForgotPassword(null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsNoContentResponse_GivenValidEmail()
        {
            //Act
            var response = await _controller.ForgotPassword("ryan.haywood@gmail.com");

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequestResponse_GivenNoPassword()
        {
            //Act
            var response = await _controller.ResetPassword(null, null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task ResetPassword_ReturnsUnprocessableEntityObjectResponse_GivenInvalidResetPassword()
        {
            //Arrange
            ResetPasswordDto resetPassword = new ResetPasswordDto();
            _controller.ModelState.AddModelError("Password", "Required");

            //Act
            var response = await _controller.ResetPassword(null, null, resetPassword);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task ResetPassword_ReturnsNotFoundResponse_GivenInvalidEmail()
        {
            //Arrange
            ResetPasswordDto resetPassword = new ResetPasswordDto();
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            //Act
            var response = await _controller.ResetPassword(null, null, resetPassword);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequestResponse_GivenInvalidPassword()
        {
            //Arrange
            ResetPasswordDto resetPassword = new ResetPasswordDto();
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "invalid password" }));

            //Act
            var response = await _controller.ResetPassword(null, null, resetPassword);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task ResetPassword_ReturnsNoContentResponse_GivenValidPassword()
        {
            //Arrange
            ResetPasswordDto resetPassword = new ResetPasswordDto();
            _mockUserManager.Setup(x => x.IsLockedOutAsync(It.IsAny<User>())).ReturnsAsync(false);

            //Act
            var response = await _controller.ResetPassword("ryan.haywood@gmail.com", "valid token", resetPassword);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequestResponse_GivenNoPassword()
        {
            //Act
            var response = await _controller.ChangePassword(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnprocessableEntityObjectResponse_GivenInvalidResetPassword()
        {
            //Arrange
            ChangedPasswordDto changedPassword = new ChangedPasswordDto();
            _controller.ModelState.AddModelError("Password", "Required");

            //Act
            var response = await _controller.ChangePassword(null, changedPassword);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ReturnsNotFoundResponse_GivenInvalidEmail()
        {
            //Arrange
            ChangedPasswordDto changedPassword = new ChangedPasswordDto();
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            //Act
            var response = await _controller.ChangePassword(null, changedPassword);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequestResponse_GivenInvalidPassword()
        {
            //Arrange
            ChangedPasswordDto changedPassword = new ChangedPasswordDto();
            _mockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "invalid password" }));

            //Act
            var response = await _controller.ChangePassword(null, changedPassword);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ReturnsNoContentResponse_GivenValidPassword()
        {
            //Arrange
            ChangedPasswordDto changedPassword = new ChangedPasswordDto();

            //Act
            var response = await _controller.ChangePassword("ryan.haywood@gmail.com", changedPassword);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsNotFound_GivenInvalidEmail()
        {
            //Arrange
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            //Act
            var response = await _controller.ConfirmEmail(null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsNoContent_GivenValidTokenAndEmail()
        {
            //Act
            var response = await _controller.ConfirmEmail(null, null);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task BlockRegistration_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = await _controller.BlockRegistration(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BlockRegistration_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("b6e2ad45-31da-4d0e-ab9f-2193dd539fc6");

            //Act
            var response = await _controller.BlockRegistration(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequestResponse_GivenNoUser()
        {
            //Act
            var response = await _controller.UpdateUser(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateUser_ReturnsUnprocessableEntityObjectResponse_GivenInvalidUser()
        {
            //Arrange
            UserUpdateDto user = new UserUpdateDto();
            _controller.ModelState.AddModelError("FirstName", "Required");

            //Act
            var response = await _controller.UpdateUser(Guid.Empty, user);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("a56b3f62-787c-49ba-b16a-cb4cb96a73f8");
            UserUpdateDto user = new UserUpdateDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = await _controller.UpdateUser(id, user);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContentResponse_GivenValidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            UserUpdateDto country = new UserUpdateDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = await _controller.UpdateUser(id, country);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateUser_UpdatesExistingUser_GivenValidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            UserUpdateDto user = new UserUpdateDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = await _controller.UpdateUser(id, user);

            //Assert
            Assert.NotNull(await _unitOfWork.UserRepository.GetById(id));
            Assert.Equal("Kara", (await _unitOfWork.UserRepository.GetById(id)).FirstName);
            Assert.Equal("Eberle", (await _unitOfWork.UserRepository.GetById(id)).LastName);
        }

        [Fact]
        public async Task PartiallyUpdateUser_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateUser(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("ef86c6f8-e838-4a74-824a-a0bd27a95d1a");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateUser_ReturnsUnprocessableEntityObjectResponse_GivenInvalidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            _controller.ModelState.AddModelError("FirstName", "Required");

            //Act
            var response = await _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateUser_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            patchDoc.Replace(u => u.FirstName, "Kara");
            patchDoc.Replace(u => u.LastName, "Eberle");

            //Act
            var response = await _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateUser_UpdatesExistingUser_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            patchDoc.Replace(u => u.FirstName, "Kara");
            patchDoc.Replace(u => u.LastName, "Eberle");

            //Act
            var response = _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.NotNull(await _unitOfWork.UserRepository.GetById(id));
            Assert.Equal("Kara", (await _unitOfWork.UserRepository.GetById(id)).FirstName);
            Assert.Equal("Eberle", (await _unitOfWork.UserRepository.GetById(id)).LastName);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("65e2c5ae-3115-467c-8efa-30323924efed");

            //Act
            var response = await _controller.DeleteUser(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContentResponse_GivenValidUserId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = await _controller.DeleteUser(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteUser_RemovesUserFromDatabase()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            await _controller.DeleteUser(id);

            //Assert
            Assert.Equal(5, (await _unitOfWork.UserRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.UserRepository.GetById(id));
        }*/
    }
}