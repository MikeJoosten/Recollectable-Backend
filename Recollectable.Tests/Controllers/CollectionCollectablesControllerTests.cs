using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Models.Collections;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
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
    public class CollectionCollectablesControllerTests : RecollectableTestBase
    {
        private readonly CollectionCollectablesController _controller;
        private readonly Mock<ICollectionService> _mockCollectionService;
        private readonly Mock<ICollectionCollectableService> _mockCollectableService;
        private readonly CollectionCollectablesResourceParameters resourceParameters;
        private readonly CollectionCollectableTestBuilder _builder;

        public CollectionCollectablesControllerTests()
        {
            _mockCollectionService = new Mock<ICollectionService>();
            _mockCollectableService = new Mock<ICollectionCollectableService>();
            _mockCollectableService.Setup(c => c.Save()).Returns(Task.FromResult(true));
            _mockCollectionService.Setup(u => u.CollectionExists(It.IsAny<Guid>())).Returns(Task.FromResult(true));

            _controller = new CollectionCollectablesController(_mockCollectableService.Object,_mockCollectionService.Object, _mapper);

            _builder = new CollectionCollectableTestBuilder();
            resourceParameters = new CollectionCollectablesResourceParameters();
            SetupTestController(_controller);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetCollectionCollectables(Guid.Empty, resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetCollectionCollectables(Guid.Empty, resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Act
            var response = await _controller.GetCollectionCollectables(Guid.Empty, resourceParameters, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectionCollectables_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = new List<CollectionCollectable>();
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsAllCollectionCollectables_GivenNoMediaType()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<CollectionCollectableDto>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsAllCollectionCollectables_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsAllCollectionCollectables_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsCollectionCollectables_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, 1, 1);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsCollectionCollectables_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, 1, 1);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .Returns(Task.FromResult(pagedList));

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCollectionCollectable(Guid.Empty, Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("72fa9097-1a05-41e0-8c9d-f6c83296c050", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "ada81922-258b-40a4-a622-06a341cb2c10")]
        [InlineData("72fa9097-1a05-41e0-8c9d-f6c83296c050", "ada81922-258b-40a4-a622-06a341cb2c10")]
        public async Task GetCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Act
            var response = await _controller.GetCollectionCollectable(new Guid(collectionId), new Guid(id), null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectionCollectable_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            var collectionCollectable = _builder.WithId(id).Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(collectionCollectable));

            //Act
            var response = await _controller.GetCollectionCollectable(collectionId, id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectionCollectable_ReturnsCollectionCollectable_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            var collectionCollectable = _builder.WithId(id).WithCountryName("Mexico").Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(collectionCollectable));

            //Act
            var response = await _controller.GetCollectionCollectable(collectionId, id, null, null) as OkObjectResult;
            var result = response.Value as CollectionCollectableDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Mexico", result.Collectable.Country.Name);
        }

        [Fact]
        public async Task GetCollectionCollectable_ReturnsCollectionCollectable_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            var collectionCollectable = _builder.WithId(id).WithCountryName("Mexico").Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(collectionCollectable));

            //Act
            var response = await _controller.GetCollectionCollectable(collectionId, id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Mexico", result.Collectable.Country.Name);
        }

        [Fact]
        public async Task GetCollectionCollectable_ReturnsCollectionCollectable_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            var collectionCollectable = _builder.WithId(id).WithCountryName("Mexico").Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(collectionCollectable));

            //Act
            var response = await _controller.GetCollectionCollectable(collectionId, id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Mexico", result.Collectable.Country.Name);
        }

        [Fact]
        public async Task CreateCollectionCollectable_ReturnsBadRequestResponse_GivenNoCollectionCollectable()
        {
            //Act
            var response = await _controller.CreateCollectionCollectable(Guid.Empty, null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCollectionCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectionCollectable()
        {
            //Arrange
            CollectionCollectableCreationDto collectable = new CollectionCollectableCreationDto();
            _controller.ModelState.AddModelError("Condition", "Required");

            //Act
            var response = await _controller.CreateCollectionCollectable(Guid.Empty, collectable, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task CreateCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            CollectionCollectableCreationDto collectable = new CollectionCollectableCreationDto();

            //Act
            var response = await _controller.CreateCollectionCollectable(Guid.Empty, collectable, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task CreateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableId()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectableCreationDto collectable = new CollectionCollectableCreationDto
            {
                CollectableId = new Guid("01d82800-e45b-4bcf-b534-4ba70caad842")
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection()));

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCollectionCollectable_ReturnsCreatedResponse_GivenValidCollectionCollectable(string mediaType)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectableCreationDto collectable = new CollectionCollectableCreationDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .Returns(Task.FromResult(new Collectable()));

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCollectionCollectable_CreatesNewCollectionCollectable_GivenAnyMediaTypeAndValidCollectionCollectable()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");
            CollectionCollectableCreationDto collectable = new CollectionCollectableCreationDto
            {
                CollectableId = collectableId
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .Returns(Task.FromResult(new Collectable { Id = collectableId }));

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, null) as CreatedAtRouteResult;
            var returnedCollectable = response.Value as CollectionCollectableDto;

            //Assert
            Assert.NotNull(returnedCollectable);
            Assert.Equal(collectableId, returnedCollectable.Collectable.Id);
        }

        [Fact]
        public async Task CreateCollectionCollectable_CreatesNewCollectionCollectable_GivenHateoasMediaTypeAndValidCollectionCollectable()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");
            CollectionCollectableCreationDto collectable = new CollectionCollectableCreationDto
            {
                CollectableId = collectableId
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .Returns(Task.FromResult(new Collectable { Id = collectableId }));

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, mediaType) as CreatedAtRouteResult;
            dynamic returnedCollectable = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCollectable);
            Assert.Equal(collectableId, returnedCollectable.Collectable.Id);
        }

        [Fact]
        public async Task BlockCollectionCollectableCreation_ReturnsConflictResponse_GivenExistingIds()
        {
            //Arrange
            _mockCollectableService
                .Setup(c => c.CollectionCollectableExists(Guid.Empty, Guid.Empty))
                .Returns(Task.FromResult(true));

            //Act
            var response = await _controller.BlockCollectionCollectableCreation(Guid.Empty, Guid.Empty) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Theory]
        [InlineData("075b78c3-0942-4dcf-b42b-91f0e1e9f009", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "1ed26874-90ba-4a2b-8121-0dedc704805b")]
        [InlineData("075b78c3-0942-4dcf-b42b-91f0e1e9f009", "1ed26874-90ba-4a2b-8121-0dedc704805b")]
        public async Task BlockCollectionCollectableCreation_ReturnsNotFoundResponse_GivenUnexistingIds(string collectionId, string id)
        {
            //Act
            var response = await _controller.BlockCollectionCollectableCreation(new Guid(collectionId), new Guid(id));

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsBadRequestResponse_GivenNoCollectionCollectable()
        {
            //Act
            var response = await _controller.UpdateCollectionCollectable(Guid.Empty, Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectionCollectable()
        {
            //Arrange
            CollectionCollectableUpdateDto collectable = new CollectionCollectableUpdateDto();
            _controller.ModelState.AddModelError("Condition", "Required");

            //Act
            var response = await _controller.UpdateCollectionCollectable(Guid.Empty, Guid.Empty, collectable);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectionId()
        {
            //Arrange
            CollectionCollectableUpdateDto collectable = new CollectionCollectableUpdateDto
            {
                CollectionId = new Guid("2406e157-a5a4-4e0b-81c9-bea446c4387d")
            };

            //Act
            var response = await _controller.UpdateCollectionCollectable(Guid.Empty, Guid.Empty, collectable);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }
        
        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableId()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectableUpdateDto collectable = new CollectionCollectableUpdateDto
            {
                CollectableId = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f")
            };

            //Act
            var response = await _controller.UpdateCollectionCollectable(collectionId, Guid.Empty, collectable);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("31232a10-516d-43a1-bc02-229dab4e6a6c", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "55ba47e1-2157-4400-ac2f-a8ecd791493b")]
        [InlineData("31232a10-516d-43a1-bc02-229dab4e6a6c", "55ba47e1-2157-4400-ac2f-a8ecd791493b")]
        public async Task UpdateCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Arrange
            CollectionCollectableUpdateDto collectable = new CollectionCollectableUpdateDto
            {
                CollectionId = new Guid(collectionId),
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(new Guid(collectionId)))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .Returns(Task.FromResult(new Collectable()));

            //Act
            var response = await _controller.UpdateCollectionCollectable(new Guid(collectionId), new Guid(id), collectable);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsNoContentResponse_GivenValidCollectionCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectableUpdateDto collectable = new CollectionCollectableUpdateDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .Returns(Task.FromResult(new Collectable()));

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));

            //Act
            var response = await _controller.UpdateCollectionCollectable(collectionId, id, collectable);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCollectionCollectable_UpdatesExistingCollectionCollectable_GivenValidCollectionCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectionCollectableUpdateDto collectable = new CollectionCollectableUpdateDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .Returns(Task.FromResult(new Collectable()));

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));
            _mockCollectableService.Setup(c => c.UpdateCollectionCollectable(It.IsAny<CollectionCollectable>()));

            //Act
            var response = await _controller.UpdateCollectionCollectable(collectionId, id, collectable);

            //Assert
            _mockCollectableService.Verify(c => c.UpdateCollectionCollectable(retrievedCollectionCollectable));
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(Guid.Empty, Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("8185e242-e8fd-4776-a618-17717702a3ed", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "31a58da0-6e0a-4d33-b54d-8c33759c0473")]
        [InlineData("8185e242-e8fd-4776-a618-17717702a3ed", "31a58da0-6e0a-4d33-b54d-8c33759c0473")]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Arrange
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(new Guid(collectionId), new Guid(id), patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectionCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            _controller.ModelState.AddModelError("Condition", "Required");

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectionId, new Guid("10ab82c7-b111-4c09-8793-e2d2d08a8178"));

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("28ee2c6b-dab3-4616-a92f-cc7138228cb8")]
        [InlineData("54826cab-0395-4304-8c2f-6c3bdc82237f")]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableId(string collectableId)
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid(collectableId));

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Collectable()));

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_UpdatesExistingCollectionCollectable_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .Returns(Task.FromResult(new Collection { Type = string.Empty }));

            _mockCollectableService
                .Setup(c => c.FindCollectableById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Collectable()));

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(retrievedCollectionCollectable));
            _mockCollectableService.Setup(c => c.UpdateCollectionCollectable(It.IsAny<CollectionCollectable>()));

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            _mockCollectableService.Verify(c => c.UpdateCollectionCollectable(retrievedCollectionCollectable));
        }

        [Theory]
        [InlineData("23fabd7c-7dd0-4db4-a517-1db5351826ab", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "4dc6a62e-c239-49db-8976-548015e1a7fd")]
        [InlineData("23fabd7c-7dd0-4db4-a517-1db5351826ab", "4dc6a62e-c239-49db-8976-548015e1a7fd")]
        public async Task DeleteCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Act
            var response = await _controller.DeleteCollectionCollectable(new Guid(collectionId), new Guid(id));

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCollectionCollectable_ReturnsNoContentResponse_GivenValidIds()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectable = _builder.Build();
            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(collectionCollectable));

            //Act
            var response = await _controller.DeleteCollectionCollectable(collectionId, id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteCollectionCollectable_RemovesCollectionCollectableFromDatabase()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectable = _builder.Build();
            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .Returns(Task.FromResult(collectionCollectable));
            _mockCollectableService
                .Setup(c => c.RemoveCollectionCollectable(It.IsAny<CollectionCollectable>()));

            //Act
            await _controller.DeleteCollectionCollectable(collectionId, id);

            //Assert
            _mockCollectableService.Verify(c => c.RemoveCollectionCollectable(collectionCollectable));
        }
    }
}