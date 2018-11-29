using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
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
        private readonly CollectionsResourceParameters resourceParameters;

        /*public CollectionsControllerTests()
        {
            _controller = new CollectionsController(_unitOfWork, _typeHelperService,
                _propertyMappingService, _mapper);

            resourceParameters = new CollectionsResourceParameters();
            SetupTestController<CollectionDto, Collection>(_controller);
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

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollections_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollections_ReturnsAllCollections_GivenNoMediaType()
        {
            //Act
            var response = await _controller.GetCollections(resourceParameters, null) as OkObjectResult;
            var collections = response.Value as List<CollectionDto>;

            //Assert
            Assert.NotNull(collections);
            Assert.Equal(6, collections.Count);
        }

        [Fact]
        public async Task GetCollections_ReturnsAllCollections_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType) as OkObjectResult;
            var collections = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(collections);
            Assert.Equal(6, collections.Count);
        }

        [Fact]
        public async Task GetCollections_ReturnsAllCollections_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public async Task GetCollections_ReturnsCollections_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType) as OkObjectResult;
            var collections = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(collections);
            Assert.Equal(2, collections.Count);
        }

        [Fact]
        public async Task GetCollections_ReturnsCollections_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetCollections(resourceParameters, mediaType) as OkObjectResult;
            var collections = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(collections);
            Assert.Equal(2, collections.Value.Count());
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
            //Arrange
            Guid id = new Guid("76efa0d4-2ec6-4dbb-80fa-a689fd5fe884");

            //Act
            var response = await _controller.GetCollection(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollection_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            //Act
            var response = await _controller.GetCollection(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollection_ReturnsCollection_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            //Act
            var response = await _controller.GetCollection(id, null, null) as OkObjectResult;
            var collection = response.Value as CollectionDto;

            //Assert
            Assert.NotNull(collection);
            Assert.Equal(id, collection.Id);
            Assert.Equal("Coin", collection.Type);
        }

        [Fact]
        public async Task GetCollection_ReturnsCollection_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            //Act
            var response = await _controller.GetCollection(id, null, mediaType) as OkObjectResult;
            dynamic collection = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(collection);
            Assert.Equal(id, collection.Id);
            Assert.Equal("Coin", collection.Type);
        }

        [Fact]
        public async Task GetCollection_ReturnsCollection_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

            //Act
            var response = await _controller.GetCollection(id, null, mediaType) as OkObjectResult;
            dynamic collection = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(collection);
            Assert.Equal(id, collection.Id);
            Assert.Equal("Coin", collection.Type);
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
            CollectionCreationDto collection = new CollectionCreationDto();
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
            CollectionCreationDto collection = new CollectionCreationDto
            {
                UserId = new Guid("e47d5063-7734-48ae-b088-0e4dca54a821")
            };

            //Act
            var response = await _controller.CreateCollection(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCollection_ReturnsCreatedResponse_GivenValidCollection(string mediaType)
        {
            //Arrange
            CollectionCreationDto collection = new CollectionCreationDto
            {
                Type = "Banknote",
                UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
            };

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
            CollectionCreationDto collection = new CollectionCreationDto
            {
                Type = "Banknote",
                UserId = userId
            };

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
            CollectionCreationDto collection = new CollectionCreationDto
            {
                Type = "Banknote",
                UserId = userId
            };

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

            //Act
            var response = await _controller.BlockCollectionCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BlockCollectionCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("514d80af-7748-46ed-99cd-a71f6d58315a");

            //Act
            var response = await _controller.BlockCollectionCreation(id);

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
            CollectionUpdateDto collection = new CollectionUpdateDto();
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
            CollectionUpdateDto collection = new CollectionUpdateDto
            {
                Type = "Banknote",
                UserId = userId
            };

            //Act
            var response = await _controller.UpdateCollection(Guid.Empty, collection);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("e69bed8a-1b96-4d74-bff6-4c2894dd7872");
            CollectionUpdateDto collection = new CollectionUpdateDto
            {
                Type = "Banknote",
                UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
            };

            //Act
            var response = await _controller.UpdateCollection(id, collection);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollection_ReturnsNoContentResponse_GivenValidCollection()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            CollectionUpdateDto collection = new CollectionUpdateDto
            {
                Type = "Banknote",
                UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
            };

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
            Guid userId = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158");
            CollectionUpdateDto country = new CollectionUpdateDto
            {
                Type = "Banknote",
                UserId = userId
            };

            //Act
            var response = await _controller.UpdateCollection(id, country);

            //Assert
            Assert.NotNull(await _unitOfWork.CollectionRepository.GetById(id));
            Assert.Equal("Banknote", (await _unitOfWork.CollectionRepository.GetById(id)).Type);
            Assert.Equal(userId, (await _unitOfWork.CollectionRepository.GetById(id)).UserId);
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
            Guid id = new Guid("9f8925ae-1bbe-49f1-98d9-193f69a7fc5c");
            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollection()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
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
            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();
            patchDoc.Replace(c => c.Type, "Banknote");
            patchDoc.Replace(c => c.UserId, userId);

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollection_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Guid userId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1");
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
            JsonPatchDocument<CollectionUpdateDto> patchDoc = new JsonPatchDocument<CollectionUpdateDto>();
            patchDoc.Replace(c => c.Type, "Banknote");
            patchDoc.Replace(c => c.UserId, userId);

            //Act
            var response = await _controller.PartiallyUpdateCollection(id, patchDoc);

            //Assert
            Assert.NotNull(await _unitOfWork.CollectionRepository.GetById(id));
            Assert.Equal("Banknote", (await _unitOfWork.CollectionRepository.GetById(id)).Type);
            Assert.Equal(userId, (await _unitOfWork.CollectionRepository.GetById(id)).UserId);
        }

        [Fact]
        public async Task DeleteCollection_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("02aef0b1-fef7-4e37-975a-0df8607c8efb");

            //Act
            var response = await _controller.DeleteCollection(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCollection_ReturnsNoContentResponse_GivenValidCollectionId()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");

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

            //Act
            await _controller.DeleteCollection(id);

            //Assert
            Assert.Equal(5, (await _unitOfWork.CollectionRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.CollectionRepository.GetById(id));
        }*/
    }
}