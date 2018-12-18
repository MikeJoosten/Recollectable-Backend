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
using Recollectable.Tests.Builders;
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
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserStore<User>> _mockUserStore;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly UsersResourceParameters resourceParameters;
        private readonly UserTestBuilder _builder;

        public UsersControllerTests()
        {
            _mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>
                (_mockUserStore.Object, null, null, null, null, null, null, null, null);

            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            _mockEmailService = new Mock<IEmailService>();
            _mockTokenFactory = new Mock<ITokenFactory>();

            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(u => u.Save()).Returns(Task.FromResult(true));

            _controller = new UsersController(_mockUserService.Object, _mockUserManager.Object, 
                _mockTokenFactory.Object, _mockEmailService.Object, _mapper);
            SetupTestController(_controller);
            SetupAuthentication(_controller);

            _builder = new UserTestBuilder();
            resourceParameters = new UsersResourceParameters();
            resourceParameters.Fields = "Id, UserName";
        }

        [Fact]
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

        [Fact]
        public async Task GetUsers_ReturnsBadRequestObjectResponse_GivenFieldParameterWithNoId()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var users = _builder.Build(2);
            var pagedList = PagedList<User>.Create(users,
                resourceParameters.Page, resourceParameters.PageSize);
            resourceParameters.Fields = "UserName";

            _mockUserService
                .Setup(u => u.FindUsers(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetUsers_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            var users = _builder.Build(2);
            var pagedList = PagedList<User>.Create(users,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockUserService
                .Setup(u => u.FindUsers(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers_GivenAnyMediaType()
        {
            //Arrange
            var users = _builder.Build(2);
            var pagedList = PagedList<User>.Create(users,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockUserService
                .Setup(u => u.FindUsers(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetUsers(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var users = _builder.Build(2);
            var pagedList = PagedList<User>.Create(users,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockUserService
                .Setup(u => u.FindUsers(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetUsers_ReturnsUsers_GivenAnyMediaTypeAndPagingParameters()
        {
            //Arrange
            var users = _builder.Build(4);
            var pagedList = PagedList<User>.Create(users, 1, 2);

            _mockUserService
                .Setup(u => u.FindUsers(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetUsers(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsUsers_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var users = _builder.Build(4);
            var pagedList = PagedList<User>.Create(users, 1, 2);

            _mockUserService
                .Setup(u => u.FindUsers(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
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
            //Act
            var response = await _controller.GetUser(Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetCoin_ReturnsBadRequestObjectResponse_GivenFieldParameterWithNoId()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var user = _builder.WithId(id).WithUserName("Ryan").Build();
            resourceParameters.Fields = "UserName";

            _mockUserService
                .Setup(u => u.FindUserById(id))
                .ReturnsAsync(user);

            //Act
            var response = await _controller.GetUser(id, resourceParameters.Fields, mediaType);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetUser_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var user = _builder.Build();

            _mockUserService
                .Setup(u => u.FindUserById(id))
                .ReturnsAsync(user);

            //Act
            var response = await _controller.GetUser(id, resourceParameters.Fields, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_GivenAnyMediaType()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var user = _builder.WithId(id).WithUserName("Ryan").Build();

            _mockUserService
                .Setup(u => u.FindUserById(id))
                .ReturnsAsync(user);

            //Act
            var response = await _controller.GetUser(id, null, null) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Ryan", result.UserName);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var user = _builder.WithId(id).WithUserName("Ryan").Build();

            _mockUserService
                .Setup(u => u.FindUserById(id))
                .ReturnsAsync(user);

            //Act
            var response = await _controller.GetUser(id, resourceParameters.Fields, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Ryan", result.UserName);
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
            var user = _builder.BuildCreationDto();
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
            var user = _builder.BuildCreationDto();
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
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
            var user = _builder.BuildCreationDto();
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
            var user = _builder.WithUserName("Kara").BuildCreationDto();

            //Act
            var response = await _controller.Register(user, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task Register_CreatesNewUser_GivenAnyMediaTypeAndValidUser()
        {
            //Arrange
            var user = _builder.WithUserName("Kara").BuildCreationDto();

            //Act
            var response = await _controller.Register(user, null) as CreatedAtRouteResult;
            var result = response.Value as UserDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Kara", result.UserName);
        }

        [Fact]
        public async Task Register_CreatesNewUser_GivenHateoasMediaTypeAndValidUser()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var user = _builder.WithUserName("Kara").BuildCreationDto();

            //Act
            var response = await _controller.Register(user, mediaType) as CreatedAtRouteResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Kara", result.UserName);
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
        public async Task Login_ReturnsNotFoundResponse_GivenInvalidUserName()
        {
            //Arrange
            var credentials = _builder.BuildCredentialsDto();
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            //Act
            var response = await _controller.Login(credentials);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Login_ReturnsBadRequestResponse_GivenInvalidPassword()
        {
            //Arrange
            var credentials = _builder.BuildCredentialsDto();
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            //Act
            var response = await _controller.Login(credentials);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Login_ReturnsOkResponse_GivenValidUser()
        {
            //Arrange
            var user = _builder.WithUserName("Ryan").Build();
            var credentials = _builder.BuildCredentialsDto("password");
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.CheckPasswordAsync(user, credentials.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            //Act
            var response = await _controller.Login(credentials);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Login_ReturnsLoginInfo_GivenValidUser()
        {
            //Arrange
            var user = _builder.WithUserName("Ryan").Build();
            var credentials = _builder.BuildCredentialsDto("password");
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.CheckPasswordAsync(user, credentials.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            //Act
            var response = await _controller.Login(credentials) as OkObjectResult;

            //Assert
            Assert.NotNull(response.Value);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsNotFoundResponse_GivenInvalidEmail()
        {
            //Arrange
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            //Act
            var response = await _controller.ForgotPassword(null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsNoContentResponse_GivenValidEmail()
        {
            //Arrange
            var user = _builder.WithEmail("ryan.haywood@gmail.com").Build();
            _mockUserManager.Setup(u => u.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            //Act
            var response = await _controller.ForgotPassword(user.Email);

            //Assert
            Assert.IsType<NoContentResult>(response);
            _mockUserManager.Verify(u => u.GeneratePasswordResetTokenAsync(user));
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
            var resetPassword = _builder.BuildResetPasswordDto();
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
            var resetPassword = _builder.BuildResetPasswordDto();
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
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
            var resetPassword = _builder.BuildResetPasswordDto();
            _mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
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
            var resetPassword = _builder.BuildResetPasswordDto();
            _mockUserManager.Setup(u => u.IsLockedOutAsync(It.IsAny<User>())).ReturnsAsync(false);

            //Act
            var response = await _controller.ResetPassword("valid token", "ryan.haywood@gmail.com", resetPassword);

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
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
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
            _mockUserManager.Setup(u => u.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
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
            var changedPassword = _builder.BuildChangedPasswordDto();

            //Act
            var response = await _controller.ChangePassword("ryan.haywood@gmail.com", changedPassword);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsNotFound_GivenInvalidEmail()
        {
            //Arrange
            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
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
            _mockUserService.Setup(u => u.UserExists(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            var response = await _controller.BlockRegistration(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            _mockUserService.Verify(u => u.UserExists(id));
        }

        [Fact]
        public async Task BlockRegistration_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Act
            var response = await _controller.BlockRegistration(Guid.Empty);

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
            var user = _builder.WithUserName("Kara").BuildUpdateDto();

            //Act
            var response = await _controller.UpdateUser(Guid.Empty, user);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContentResponse_GivenValidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var user = _builder.WithUserName("Kara").BuildUpdateDto();
            var retrievedUser = _builder.Build();

            _mockUserService.Setup(u => u.FindUserById(id)).ReturnsAsync(retrievedUser);

            //Act
            var response = await _controller.UpdateUser(id, user);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateUser_UpdatesExistingUser_GivenValidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var user = _builder.WithUserName("Kara").BuildUpdateDto();
            var retrievedUser = _builder.Build();

            _mockUserService.Setup(u => u.FindUserById(id)).ReturnsAsync(retrievedUser);
            _mockUserService.Setup(u => u.UpdateUser(It.IsAny<User>()));

            //Act
            var response = await _controller.UpdateUser(id, user);

            //Assert
            _mockUserService.Verify(u => u.UpdateUser(retrievedUser));
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
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateUser(Guid.Empty, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateUser_ReturnsUnprocessableEntityObjectResponse_GivenInvalidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            var user = _builder.Build();
            _mockUserService.Setup(c => c.FindUserById(id)).ReturnsAsync(user);

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

            var user = _builder.Build();
            _mockUserService.Setup(c => c.FindUserById(id)).ReturnsAsync(user);

            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            patchDoc.Replace(u => u.UserName, "Kara");

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

            var user = _builder.Build();
            _mockUserService.Setup(c => c.FindUserById(id)).ReturnsAsync(user);
            _mockUserService.Setup(c => c.UpdateUser(It.IsAny<User>()));

            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            patchDoc.Replace(u => u.UserName, "Kara");

            //Act
            var response = await _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            _mockUserService.Verify(c => c.UpdateUser(user));
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Act
            var response = await _controller.DeleteUser(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContentResponse_GivenValidUserId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            var user = _builder.Build();
            _mockUserService.Setup(c => c.FindUserById(id)).ReturnsAsync(user);

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

            var user = _builder.Build();
            _mockUserService.Setup(c => c.FindUserById(id)).ReturnsAsync(user);
            _mockUserService.Setup(c => c.RemoveUser(It.IsAny<User>()));

            //Act
            await _controller.DeleteUser(id);

            //Assert
            _mockUserService.Verify(c => c.RemoveUser(user));
        }
    }
}