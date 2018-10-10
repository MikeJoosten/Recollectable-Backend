using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class UsersControllerTests : RecollectableTestBase
    {
        private readonly UsersController _controller;
        private readonly UsersResourceParameters resourceParameters;

        public UsersControllerTests()
        {
            _controller = new UsersController(_unitOfWork, _typeHelperService,
                _propertyMappingService, _mapper);

            resourceParameters = new UsersResourceParameters();
            SetupTestController<UserDto, User>(_controller);
        }

        [Fact]
        public void GetUsers_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = _controller.GetUsers(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void GetUsers_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = _controller.GetUsers(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public void GetUsers_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = _controller.GetUsers(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetUsers_ReturnsAllUsers_GivenNoMediaType()
        {
            //Act
            var response = _controller.GetUsers(resourceParameters, null) as OkObjectResult;
            var users = response.Value as List<UserDto>;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(6, users.Count);
        }

        [Fact]
        public void GetUsers_ReturnsAllUsers_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var users = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(6, users.Count);
        }

        [Fact]
        public void GetUsers_ReturnsAllUsers_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public void GetUsers_ReturnsUsers_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var users = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void GetUsers_ReturnsUsers_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = _controller.GetUsers(resourceParameters, mediaType) as OkObjectResult;
            var users = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Value.Count());
        }

        [Fact]
        public void GetUser_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = _controller.GetUser(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void GetUser_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("72e2cde4-0aec-47e7-9549-f2c578b2c21c");

            //Act
            var response = _controller.GetUser(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public void GetUser_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = _controller.GetUser(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetUser_ReturnsUser_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = _controller.GetUser(id, null, null) as OkObjectResult;
            var user = response.Value as UserDto;

            //Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal("Ryan Haywood", user.Name);
        }

        [Fact]
        public void GetUser_ReturnsUser_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = _controller.GetUser(id, null, mediaType) as OkObjectResult;
            dynamic user = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal("Ryan Haywood", user.Name);
        }

        [Fact]
        public void GetUser_ReturnsUser_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = _controller.GetUser(id, null, mediaType) as OkObjectResult;
            dynamic user = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(user);
            Assert.Equal(id, user.Id);
            Assert.Equal("Ryan Haywood", user.Name);
        }

        [Fact]
        public void CreateUser_ReturnsBadRequestResponse_GivenNoUser()
        {
            //Act
            var response = _controller.CreateUser(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void CreateUser_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            UserCreationDto user = new UserCreationDto();
            _controller.ModelState.AddModelError("FirstName", "Required");

            //Act
            var response = _controller.CreateUser(user, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public void CreateUser_ReturnsCreatedResponse_GivenValidUser(string mediaType)
        {
            //Arrange
            UserCreationDto user = new UserCreationDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = _controller.CreateUser(user, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public void CreateUser_CreatesNewUser_GivenAnyMediaTypeAndValidUser()
        {
            //Arrange
            UserCreationDto user = new UserCreationDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = _controller.CreateUser(user, null) as CreatedAtRouteResult;
            var returnedUser = response.Value as UserDto;

            //Assert
            Assert.NotNull(returnedUser);
            Assert.Equal("Kara Eberle", returnedUser.Name);
        }

        [Fact]
        public void CreateUser_CreatesNewUser_GivenHateoasMediaTypeAndValidUser()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            UserCreationDto user = new UserCreationDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = _controller.CreateUser(user, mediaType) as CreatedAtRouteResult;
            dynamic returnedUser = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedUser);
            Assert.Equal("Kara Eberle", returnedUser.Name);
        }

        [Fact]
        public void BlockUserCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = _controller.BlockUserCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public void BlockUserCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("b6e2ad45-31da-4d0e-ab9f-2193dd539fc6");

            //Act
            var response = _controller.BlockUserCreation(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void UpdateUser_ReturnsBadRequestResponse_GivenNoUser()
        {
            //Act
            var response = _controller.UpdateUser(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateUser_ReturnsUnprocessableEntityObjectResponse_GivenInvalidUser()
        {
            //Arrange
            UserUpdateDto user = new UserUpdateDto();
            _controller.ModelState.AddModelError("FirstName", "Required");

            //Act
            var response = _controller.UpdateUser(Guid.Empty, user);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void UpdateUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("a56b3f62-787c-49ba-b16a-cb4cb96a73f8");
            UserUpdateDto user = new UserUpdateDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = _controller.UpdateUser(id, user);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void UpdateUser_ReturnsNoContentResponse_GivenValidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            UserUpdateDto country = new UserUpdateDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = _controller.UpdateUser(id, country);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void UpdateUser_UpdatesExistingUser_GivenValidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            UserUpdateDto user = new UserUpdateDto
            {
                FirstName = "Kara",
                LastName = "Eberle"
            };

            //Act
            var response = _controller.UpdateUser(id, user);

            //Assert
            Assert.NotNull(_unitOfWork.UserRepository.GetById(id));
            Assert.Equal("Kara", _unitOfWork.UserRepository.GetById(id).FirstName);
            Assert.Equal("Eberle", _unitOfWork.UserRepository.GetById(id).LastName);
        }

        [Fact]
        public void PartiallyUpdateUser_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = _controller.PartiallyUpdateUser(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("ef86c6f8-e838-4a74-824a-a0bd27a95d1a");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();

            //Act
            var response = _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void PartiallyUpdateUser_ReturnsUnprocessableEntityObjectResponse_GivenInvalidUser()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            _controller.ModelState.AddModelError("FirstName", "Required");

            //Act
            var response = _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void PartiallyUpdateUser_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            patchDoc.Replace(u => u.FirstName, "Kara");
            patchDoc.Replace(u => u.LastName, "Eberle");

            //Act
            var response = _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void PartiallyUpdateUser_UpdatesExistingUser_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            JsonPatchDocument<UserUpdateDto> patchDoc = new JsonPatchDocument<UserUpdateDto>();
            patchDoc.Replace(u => u.FirstName, "Kara");
            patchDoc.Replace(u => u.LastName, "Eberle");

            //Act
            var response = _controller.PartiallyUpdateUser(id, patchDoc);

            //Assert
            Assert.NotNull(_unitOfWork.UserRepository.GetById(id));
            Assert.Equal("Kara", _unitOfWork.UserRepository.GetById(id).FirstName);
            Assert.Equal("Eberle", _unitOfWork.UserRepository.GetById(id).LastName);
        }

        [Fact]
        public void DeleteUser_ReturnsNotFoundResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("65e2c5ae-3115-467c-8efa-30323924efed");

            //Act
            var response = _controller.DeleteUser(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void DeleteUser_ReturnsNoContentResponse_GivenValidUserId()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            var response = _controller.DeleteUser(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void DeleteUser_RemovesUserFromDatabase()
        {
            //Arrange
            Guid id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            //Act
            _controller.DeleteUser(id);

            //Assert
            Assert.Equal(5, _unitOfWork.UserRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.UserRepository.GetById(id));
        }
    }
}