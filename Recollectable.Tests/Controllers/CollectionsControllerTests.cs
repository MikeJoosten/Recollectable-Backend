using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Models.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class CollectionsControllerTests : RecollectableTestBase
    {
        private readonly CollectionsController _controller;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ICollectionService> _mockCollectionService;
        private readonly CollectionsResourceParameters resourceParameters;
        private readonly CollectionTestBuilder _builder;

        public CollectionsControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCollectionService = new Mock<ICollectionService>();
            _mockCollectionService.Setup(c => c.Save()).ReturnsAsync(true);
            _mockUserService.Setup(u => u.UserExists(It.IsAny<Guid>())).ReturnsAsync(true);

            _controller = new CollectionsController(_mockCollectionService.Object, _mockUserService.Object, _mapper);
            SetupTestController(_controller);

            _builder = new CollectionTestBuilder();
            resourceParameters = new CollectionsResourceParameters();
            resourceParameters.Fields = "Id, Type";
        }

        [Fact]
        public async Task GetCollections_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetCollections(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollections_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetCollections(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollections_ReturnsBadRequestObjectResponse_GivenFieldParameterWithNoId()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var collections = _builder.Build(2);
            var pagedList = PagedList<Collection>.Create(collections,
                resourceParameters.Page, resourceParameters.PageSize);
            resourceParameters.Fields = "Type";

            _mockCollectionService
                .Setup(c => c.FindCollections(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetCollections_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            var collections = _builder.Build(2);
            var pagedList = PagedList<Collection>.Create(collections, 
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectionService
                .Setup(c => c.FindCollections(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollections_ReturnsAllCollections_GivenAnyMediaType()
        {
            //Arrange
            var collections = _builder.Build(2);
            var pagedList = PagedList<Collection>.Create(collections,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectionService
                .Setup(c => c.FindCollections(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollections(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCollections_ReturnsAllCollections_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var collections = _builder.Build(2);
            var pagedList = PagedList<Collection>.Create(collections,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectionService
                .Setup(c => c.FindCollections(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCollections_ReturnsCollections_GivenAnyMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            var collections = _builder.Build(4);
            var pagedList = PagedList<Collection>.Create(collections, 1, 2);

            _mockCollectionService
                .Setup(c => c.FindCollections(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollections(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCollections_ReturnsCollections_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var collections = _builder.Build(4);
            var pagedList = PagedList<Collection>.Create(collections, 1, 2);

            _mockCollectionService
                .Setup(c => c.FindCollections(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCollection_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCollection(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollection_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Act
            var response = await _controller.GetCollection(Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetCollection_ReturnsBadRequestObjectResponse_GivenFieldParameterWithNoId()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            var collection = _builder.WithId(id).WithType("Coin").Build();
            resourceParameters.Fields = "Type";

            _mockCollectionService
                .Setup(c => c.FindCollectionById(id))
                .ReturnsAsync(collection);

            //Act
            var response = await _controller.GetCollection(id, resourceParameters.Fields, mediaType);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetCollection_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            var collection = _builder.Build();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(id))
                .ReturnsAsync(collection);

            //Act
            var response = await _controller.GetCollection(id, resourceParameters.Fields, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollection_ReturnsCollection_GivenAnyMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            var collection = _builder.WithId(id).WithType("Coin").Build();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(id))
                .ReturnsAsync(collection);

            //Act
            var response = await _controller.GetCollection(id, null, null) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Coin", result.Type);
        }

        [Fact]
        public async Task GetCollection_ReturnsCollection_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            var collection = _builder.WithId(id).WithType("Coin").Build();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(id))
                .ReturnsAsync(collection);

            //Act
            var response = await _controller.GetCollection(id, resourceParameters.Fields, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Coin", result.Type);
        }

        [Fact]
        public async Task CreateCollection_ReturnsBadRequestResponse_GivenNoCollection()
        {
            //Act
            var response = await _controller.CreateCollection(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCollection_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollection()
        {
            //Arrange
            var collection = _builder.BuildCreationDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.CreateCollection(collection, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task CreateCollection_ReturnsBadRequestResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid userId = new Guid("e47d5063-7734-48ae-b088-0e4dca54a821");
            var collection = _builder.WithUserId(userId).BuildCreationDto();
            _mockUserService.Setup(u => u.FindUserById(It.IsAny<Guid>())).ReturnsAsync(null as User);

            //Act
            var response = await _controller.CreateCollection(collection, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockUserService.Verify(u => u.FindUserById(userId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCollection_ReturnsCreatedResponse_GivenValidCollection(string mediaType)
        {
            //Arrange
            Guid userId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var collection = _builder.WithType("Banknote").WithUserId(userId).BuildCreationDto();
            _mockUserService.Setup(u => u.FindUserById(userId)).ReturnsAsync(new User());

            //Act
            var response = await _controller.CreateCollection(collection, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCollection_CreatesNewCollection_GivenAnyMediaTypeAndValidCollection()
        {
            //Arrange
            Guid userId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var collection = _builder.WithType("Banknote").WithUserId(userId).BuildCreationDto();
            _mockUserService.Setup(u => u.FindUserById(userId)).Returns(Task.FromResult(new User()));

            //Act
            var response = await _controller.CreateCollection(collection, null) as CreatedAtRouteResult;
            var returnedCollection = response.Value as CollectionDto;

            //Assert
            Assert.NotNull(returnedCollection);
            Assert.Equal("Banknote", returnedCollection.Type);
            Assert.Equal(userId, returnedCollection.UserId);
        }

        [Fact]
        public async Task CreateCollection_CreatesNewCollection_GivenHateoasMediaTypeAndValidCollection()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid userId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
            var collection = _builder.WithType("Banknote").WithUserId(userId).BuildCreationDto();
            _mockUserService.Setup(u => u.FindUserById(userId)).Returns(Task.FromResult(new User()));

            //Act
            var response = await _controller.CreateCollection(collection, mediaType) as CreatedAtRouteResult;
            dynamic returnedCollection = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCollection);
            Assert.Equal("Banknote", returnedCollection.Type);
            Assert.Equal(userId, returnedCollection.UserId);
        }

        [Fact]
        public async Task BlockCollectionCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            _mockCollectionService.Setup(c => c.CollectionExists(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            var response = await _controller.BlockCollectionCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            _mockCollectionService.Verify(c => c.CollectionExists(id));
        }

        [Fact]
        public async Task BlockCollectionCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Act
            var response = await _controller.BlockCollectionCreation(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_ReturnsBadRequestResponse_GivenNoCollection()
        {
            //Act
            var response = await _controller.UpdateCollection(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollection()
        {
            //Arrange
            var collection = _builder.BuildUpdateDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.UpdateCollection(Guid.Empty, collection);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_ReturnsBadRequestResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid userId = new Guid("4512e2d5-779f-4423-b117-903f85990d4a");
            var collection = _builder.WithType("Banknote").WithUserId(userId).BuildUpdateDto();
            _mockUserService.Setup(u => u.UserExists(It.IsAny<Guid>())).ReturnsAsync(false);

            //Act
            var response = await _controller.UpdateCollection(Guid.Empty, collection);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockUserService.Verify(u => u.UserExists(userId));
        }

        [Fact]
        public async Task UpdateCollection_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            var collection = _builder.WithType("Banknote").BuildUpdateDto();

            //Act
            var response = await _controller.UpdateCollection(Guid.Empty, collection);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_ReturnsNoContentResponse_GivenValidCollection()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            var collection = _builder.WithType("Banknote").BuildUpdateDto();
            var retrievedCollection = _builder.Build();

            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(retrievedCollection);

            //Act
            var response = await _controller.UpdateCollection(id, collection);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_UpdatesExistingCollection_GivenValidCollection()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            var collection = _builder.WithType("Banknote").BuildUpdateDto();
            var retrievedCollection = _builder.WithId(id).Build();

            _mockCollectionService.Setup(c => c.FindCollectionById(id)).Returns(Task.FromResult(retrievedCollection));
            _mockCollectionService.Setup(c => c.UpdateCollection(It.IsAny<Collection>()));

            //Act
            var response = await _controller.UpdateCollection(id, collection);

            //Assert
            _mockCollectionService.Verify(c => c.UpdateCollection(retrievedCollection));
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCollection(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCollection(Guid.Empty, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollection()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            var collection = _builder.Build();
            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(collection);

            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsBadRequestResponse_GivenInvalidUserId()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Guid userId = new Guid("01156051-42e2-45f7-9a7e-58100d5d8c35");

            var collection = _builder.Build();
            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(collection);
            _mockUserService.Setup(u => u.UserExists(It.IsAny<Guid>())).ReturnsAsync(false);

            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();
            patchDoc.Replace(c => c.Type, "Banknote");
            patchDoc.Replace(c => c.UserId, userId);

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockUserService.Verify(u => u.UserExists(userId));
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Guid userId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");

            var collection = _builder.Build();
            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(collection);

            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();
            patchDoc.Replace(c => c.Type, "Banknote");
            patchDoc.Replace(c => c.UserId, userId);

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollection_UpdatesExistingCollection_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Guid userId = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158");

            var collection = _builder.Build();
            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(collection);

            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();
            patchDoc.Replace(c => c.Type, "Banknote");
            patchDoc.Replace(c => c.UserId, userId);

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            _mockCollectionService.Verify(c => c.UpdateCollection(collection));
        }

        [Fact]
        public async Task DeleteCollection_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Act
            var response = await _controller.DeleteCollection(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCollection_ReturnsNoContentResponse_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            var collection = _builder.Build();
            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(collection);

            //Act
            var response = await _controller.DeleteCollection(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteCollection_RemovesCollectionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            var collection = _builder.Build();
            _mockCollectionService.Setup(c => c.FindCollectionById(id)).ReturnsAsync(collection);
            _mockCollectionService.Setup(c => c.RemoveCollection(It.IsAny<Collection>()));

            //Act
            await _controller.DeleteCollection(id);

            //Assert
            _mockCollectionService.Verify(c => c.RemoveCollection(collection));
        }
    }
}