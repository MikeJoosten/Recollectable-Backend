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
            _mockCollectableService.Setup(c => c.Save()).ReturnsAsync(true);
            _mockCollectionService.Setup(u => u.CollectionExists(It.IsAny<Guid>())).ReturnsAsync(true);

            _controller = new CollectionCollectablesController(_mockCollectableService.Object, _mockCollectionService.Object, _mapper);
            SetupTestController(_controller);

            _builder = new CollectionCollectableTestBuilder();
            resourceParameters = new CollectionCollectablesResourceParameters();
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
        [InlineData("application/json+hateoas")]
        public async Task GetCollectionCollectables_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsAllCollectionCollectables_GivenAnyMediaType()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(2);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, null) as OkObjectResult;
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
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsCollectionCollectables_GivenAnyMediaTypeAndPagingParameters()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(4);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, 1, 2);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCollectionCollectables_ReturnsCollectionCollectables_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var collectionCollectables = _builder.Build(4);
            var pagedList = PagedList<CollectionCollectable>.Create(collectionCollectables, 1, 2);

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectables(collectionId, resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCollectionCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
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

        [Fact]
        public async Task GetCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds()
        {
            //Act
            var response = await _controller.GetCollectionCollectable(Guid.Empty, Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectionCollectable_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            var collectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .ReturnsAsync(collectionCollectable);

            //Act
            var response = await _controller.GetCollectionCollectable(collectionId, id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectionCollectable_ReturnsCollectionCollectable_GivenAnyMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            var collectionCollectable = _builder.WithId(id).WithCountryName("Mexico").Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .ReturnsAsync(collectionCollectable);

            //Act
            var response = await _controller.GetCollectionCollectable(collectionId, id, null, null) as OkObjectResult;
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
                .ReturnsAsync(collectionCollectable);

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
            var collectable = _builder.BuildCreationDto();
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
            Guid collectionId = new Guid("2406e157-a5a4-4e0b-81c9-bea446c4387d");
            var collectable = _builder.BuildCreationDto();
            _mockCollectionService.Setup(c => c.FindCollectionById(It.IsAny<Guid>())).ReturnsAsync(null as Collection);

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
            _mockCollectionService.Verify(c => c.FindCollectionById(collectionId));
        }

        [Fact]
        public async Task CreateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableId()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("ac28e6d4-9698-4111-963d-b636a7b0bce0");
            var collectable = _builder.WithCollectableId(collectableId).BuildCreationDto();

            _mockCollectionService.Setup(c => c.FindCollectionById(collectionId)).ReturnsAsync(new Collection());
            _mockCollectableService.Setup(c => c.FindCollectableById(It.IsAny<Guid>())).ReturnsAsync(null as Collectable);

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockCollectableService.Verify(c => c.FindCollectableById(collectableId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCollectionCollectable_ReturnsCreatedResponse_GivenValidCollectionCollectable(string mediaType)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");
            var collectable = _builder.WithCollectableId(collectableId).BuildCreationDto();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectableId))
                .ReturnsAsync(new Collectable());

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
            var collectable = _builder.WithCollectableId(collectableId).BuildCreationDto();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectableId))
                .ReturnsAsync(new Collectable { Id = collectableId });

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, null) as CreatedAtRouteResult;
            var result = response.Value as CollectionCollectableDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(collectableId, result.Collectable.Id);
        }

        [Fact]
        public async Task CreateCollectionCollectable_CreatesNewCollectionCollectable_GivenHateoasMediaTypeAndValidCollectionCollectable()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");
            var collectable = _builder.WithCollectableId(collectableId).BuildCreationDto();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectableId))
                .ReturnsAsync(new Collectable { Id = collectableId });

            //Act
            var response = await _controller.CreateCollectionCollectable(collectionId, collectable, mediaType) as CreatedAtRouteResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(collectableId, result.Collectable.Id);
        }

        [Fact]
        public async Task BlockCollectionCollectableCreation_ReturnsConflictResponse_GivenExistingIds()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            _mockCollectableService
                .Setup(c => c.CollectionCollectableExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            //Act
            var response = await _controller.BlockCollectionCollectableCreation(collectionId, id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            _mockCollectableService.Verify(c => c.CollectionCollectableExists(collectionId, id));
        }

        [Fact]
        public async Task BlockCollectionCollectableCreation_ReturnsNotFoundResponse_GivenUnexistingIds()
        {
            //Act
            var response = await _controller.BlockCollectionCollectableCreation(Guid.Empty, Guid.Empty);

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
            Guid collectionId = new Guid("2406e157-a5a4-4e0b-81c9-bea446c4387d");
            var collectable = _builder.BuildUpdateDto();
            _mockCollectionService.Setup(c => c.FindCollectionById(It.IsAny<Guid>())).ReturnsAsync(null as Collection);

            //Act
            var response = await _controller.UpdateCollectionCollectable(collectionId, Guid.Empty, collectable);

            //Assert
            Assert.IsType<NotFoundResult>(response);
            _mockCollectionService.Verify(c => c.FindCollectionById(collectionId));
        }
        
        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableId()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");
            var collectable = _builder.WithCollectableId(collectableId).BuildUpdateDto();

            _mockCollectionService.Setup(c => c.FindCollectionById(collectionId)).ReturnsAsync(new Collection());
            _mockCollectableService.Setup(c => c.FindCollectableById(It.IsAny<Guid>())).ReturnsAsync(null as Collectable);

            //Act
            var response = await _controller.UpdateCollectionCollectable(collectionId, Guid.Empty, collectable);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockCollectableService.Verify(c => c.FindCollectableById(collectableId));
        }

        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds()
        {
            //Arrange
            Guid id = new Guid("55ba47e1-2157-4400-ac2f-a8ecd791493b");
            Guid collectionId = new Guid("31232a10-516d-43a1-bc02-229dab4e6a6c");
            Guid collectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");
            var collectable = _builder.WithCollectableId(collectableId).BuildUpdateDto();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectableId))
                .ReturnsAsync(new Collectable());

            //Act
            var response = await _controller.UpdateCollectionCollectable(collectionId, id, collectable);

            //Assert
            Assert.IsType<NotFoundResult>(response);
            _mockCollectableService.Verify(c => c.FindCollectionCollectableById(collectionId, id));
        }

        [Fact]
        public async Task UpdateCollectionCollectable_ReturnsNoContentResponse_GivenValidCollectionCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3");
            var collectable = _builder.WithCollectableId(collectableId).BuildUpdateDto();

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(collectable.CollectableId))
                .ReturnsAsync(new Collectable());

            var retrievedCollectionCollectable = _builder.Build();

            _mockCollectableService
                .Setup(c => c.FindCollectionCollectableById(collectionId, id))
                .ReturnsAsync(retrievedCollectionCollectable);

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

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds()
        {
            //Arrange
            Guid id = new Guid("27b8cc5e-ca1f-458c-a946-7d1785538d47");
            Guid collectionId = new Guid("88276b78-a257-48c3-8d22-2e1a8ee078cf");
            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
            _mockCollectableService.Verify(c => c.FindCollectionCollectableById(collectionId, id));
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectionCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            var retrievedCollectionCollectable = _builder.Build();
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(retrievedCollectionCollectable);

            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            _controller.ModelState.AddModelError("Condition", "Required");

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
            Guid invalidCollectionId = new Guid("10ab82c7-b111-4c09-8793-e2d2d08a8178");

            var retrievedCollectionCollectable = _builder.Build();
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(retrievedCollectionCollectable);

            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectionId, invalidCollectionId);

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
            _mockCollectionService.Verify(c => c.FindCollectionById(invalidCollectionId));
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableId()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            Guid collectableId = new Guid("28ee2c6b-dab3-4616-a92f-cc7138228cb8");

            var retrievedCollectionCollectable = _builder.Build();
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(retrievedCollectionCollectable);
            _mockCollectionService.Setup(c => c.FindCollectionById(collectionId)).ReturnsAsync(new Collection());

            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, collectableId);

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
            _mockCollectableService.Verify(c => c.FindCollectableById(collectableId));
        }

        [Fact]
        public async Task PartiallyUpdateCollectionCollectable_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(It.IsAny<Guid>()))
                .ReturnsAsync(new Collectable());

            var retrievedCollectionCollectable = _builder.Build();
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(retrievedCollectionCollectable);

            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));

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

            _mockCollectionService
                .Setup(c => c.FindCollectionById(collectionId))
                .ReturnsAsync(new Collection { Type = string.Empty });

            _mockCollectableService
                .Setup(c => c.FindCollectableById(It.IsAny<Guid>()))
                .ReturnsAsync(new Collectable());

            var retrievedCollectionCollectable = _builder.Build();
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(retrievedCollectionCollectable);
            _mockCollectableService.Setup(c => c.UpdateCollectionCollectable(It.IsAny<CollectionCollectable>()));

            JsonPatchDocument<CollectionCollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectionCollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));

            //Act
            var response = await _controller.PartiallyUpdateCollectionCollectable(collectionId, id, patchDoc);

            //Assert
            _mockCollectableService.Verify(c => c.UpdateCollectionCollectable(retrievedCollectionCollectable));
        }

        [Fact]
        public async Task DeleteCollectionCollectable_ReturnsNotFoundResponse_GivenInvalidIds()
        {
            //Act
            var response = await _controller.DeleteCollectionCollectable(Guid.Empty, Guid.Empty);

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
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(collectionCollectable);

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
            _mockCollectableService.Setup(c => c.FindCollectionCollectableById(collectionId, id)).ReturnsAsync(collectionCollectable);
            _mockCollectableService.Setup(c => c.RemoveCollectionCollectable(It.IsAny<CollectionCollectable>()));

            //Act
            await _controller.DeleteCollectionCollectable(collectionId, id);

            //Assert
            _mockCollectableService.Verify(c => c.RemoveCollectionCollectable(collectionCollectable));
        }
    }
}