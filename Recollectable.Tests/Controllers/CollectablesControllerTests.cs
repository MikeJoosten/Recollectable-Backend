using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class CollectablesControllerTests : RecollectableTestBase
    {
        //private readonly CollectablesController _controller;
        private readonly CollectionCollectablesResourceParameters resourceParameters;

        /*public CollectablesControllerTests()
        {
            _controller = new CollectablesController(_unitOfWork, _typeHelperService,
                _propertyMappingService, _mapper);

            resourceParameters = new CollectablesResourceParameters();
            SetupTestController<CollectableDto, CollectionCollectable>(_controller);
        }

        [Fact]
        public async Task GetCollectables_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetCollectables(Guid.Empty, resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollectables_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetCollectables(Guid.Empty, resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollectables_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid collectionId = new Guid("1fac36e1-7654-445c-bf35-9c85a6520ecd");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectables_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectables_ReturnsAllCollectables_GivenNoMediaType()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, null) as OkObjectResult;
            var collectables = response.Value as List<CollectableDto>;

            //Assert
            Assert.NotNull(collectables);
            Assert.Equal(2, collectables.Count);
        }

        [Fact]
        public async Task GetCollectables_ReturnsAllCollectables_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var collectables = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(collectables);
            Assert.Equal(2, collectables.Count);
        }

        [Fact]
        public async Task GetCollectables_ReturnsAllCollectables_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(2, linkedCollection.Value.Count());
        }

        [Fact]
        public async Task GetCollectables_ReturnsCollectables_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 1;
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var collectables = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(collectables);
            Assert.Single(collectables);
        }

        [Fact]
        public async Task GetCollectables_ReturnsCollectables_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 1;
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectables(collectionId, resourceParameters, mediaType) as OkObjectResult;
            var collectables = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(collectables);
            Assert.Single(collectables.Value);
        }

        [Fact]
        public async Task GetCollectable_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCollectable(Guid.Empty, Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("72fa9097-1a05-41e0-8c9d-f6c83296c050", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "ada81922-258b-40a4-a622-06a341cb2c10")]
        [InlineData("72fa9097-1a05-41e0-8c9d-f6c83296c050", "ada81922-258b-40a4-a622-06a341cb2c10")]
        public async Task GetCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Act
            var response = await _controller.GetCollectable(new Guid(collectionId), new Guid(id), null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectable_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectable(collectionId, id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectable_ReturnsCollectable_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectable(collectionId, id, null, null) as OkObjectResult;
            var collectable = response.Value as CollectableDto;

            //Assert
            Assert.NotNull(collectable);
            Assert.Equal(id, collectable.Id);
            Assert.Equal("United States of America", collectable.Collectable.Country.Name);
        }

        [Fact]
        public async Task GetCollectable_ReturnsCollectable_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectable(collectionId, id, null, mediaType) as OkObjectResult;
            dynamic collectable = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(collectable);
            Assert.Equal(id, collectable.Id);
            Assert.Equal("United States of America", collectable.Collectable.Country.Name);
        }

        [Fact]
        public async Task GetCollectable_ReturnsCollectable_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.GetCollectable(collectionId, id, null, mediaType) as OkObjectResult;
            dynamic collectable = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(collectable);
            Assert.Equal(id, collectable.Id);
            Assert.Equal("United States of America", collectable.Collectable.Country.Name);
        }

        [Fact]
        public async Task CreateCollectable_ReturnsBadRequestResponse_GivenNoCollectable()
        {
            //Act
            var response = await _controller.CreateCollectable(Guid.Empty, null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectable()
        {
            //Arrange
            CollectableCreationDto collectable = new CollectableCreationDto();
            _controller.ModelState.AddModelError("Condition", "Required");

            //Act
            var response = await _controller.CreateCollectable(Guid.Empty, collectable, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task CreateCollectable_ReturnsNotFoundResponse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid collectionId = new Guid("584ecf52-9c9c-42ab-83c6-f70ec197213d");
            CollectableCreationDto collectable = new CollectableCreationDto();

            //Act
            var response = await _controller.CreateCollectable(collectionId, collectable, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData("01d82800-e45b-4bcf-b534-4ba70caad842")]
        [InlineData("54826cab-0395-4304-8c2f-6c3bdc82237f")]
        public async Task CreateCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableItemId(string collectableItemId)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableCreationDto collectable = new CollectableCreationDto
            {
                CollectableId = new Guid(collectableItemId)
            };

            //Act
            var response = await _controller.CreateCollectable(collectionId, collectable, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCollectable_ReturnsCreatedResponse_GivenValidCollectable(string mediaType)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableCreationDto collectable = new CollectableCreationDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            //Act
            var response = await _controller.CreateCollectable(collectionId, collectable, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCollectable_CreatesNewCollectable_GivenAnyMediaTypeAndValidCollectable()
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableCreationDto collectable = new CollectableCreationDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            //Act
            var response = await _controller.CreateCollectable(collectionId, collectable, null) as CreatedAtRouteResult;
            var returnedCollectable = response.Value as CollectableDto;

            //Assert
            Assert.NotNull(returnedCollectable);
            Assert.Equal("Mexico", returnedCollectable.Collectable.Country.Name);
        }

        [Fact]
        public async Task CreateCollectable_CreatesNewCollectable_GivenHateoasMediaTypeAndValidCollectable()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableCreationDto collectable = new CollectableCreationDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            //Act
            var response = await _controller.CreateCollectable(collectionId, collectable, mediaType) as CreatedAtRouteResult;
            dynamic returnedCollectable = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCollectable);
            Assert.Equal("Mexico", returnedCollectable.Collectable.Country.Name);
        }

        [Fact]
        public async Task BlockCollectableCreation_ReturnsConflictResponse_GivenExistingIds()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.BlockCollectableCreation(collectionId, id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Theory]
        [InlineData("075b78c3-0942-4dcf-b42b-91f0e1e9f009", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "1ed26874-90ba-4a2b-8121-0dedc704805b")]
        [InlineData("075b78c3-0942-4dcf-b42b-91f0e1e9f009", "1ed26874-90ba-4a2b-8121-0dedc704805b")]
        public async Task BlockCollectableCreation_ReturnsNotFoundResponse_GivenUnexistingIds(string collectionId, string id)
        {
            //Act
            var response = await _controller.BlockCollectableCreation(new Guid(collectionId), new Guid(id));

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollectable_ReturnsBadRequestResponse_GivenNoCollectable()
        {
            //Act
            var response = await _controller.UpdateCollectable(Guid.Empty, Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectable()
        {
            //Arrange
            CollectableUpdateDto collectable = new CollectableUpdateDto();
            _controller.ModelState.AddModelError("Condition", "Required");

            //Act
            var response = await _controller.UpdateCollectable(Guid.Empty, Guid.Empty, collectable);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCollectable_ReturnsBadRequestResponse_GivenInvalidCollectionId()
        {
            //Arrange
            CollectableUpdateDto collectable = new CollectableUpdateDto
            {
                CollectionId = new Guid("2406e157-a5a4-4e0b-81c9-bea446c4387d")
            };

            //Act
            var response = await _controller.UpdateCollectable(Guid.Empty, Guid.Empty, collectable);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("ac28e6d4-9698-4111-963d-b636a7b0bce0")]
        [InlineData("54826cab-0395-4304-8c2f-6c3bdc82237f")]
        public async Task UpdateCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableItemId(string collectableItemId)
        {
            //Arrange
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableUpdateDto collectable = new CollectableUpdateDto
            {
                CollectableId = new Guid(collectableItemId)
            };

            //Act
            var response = await _controller.UpdateCollectable(collectionId, Guid.Empty, collectable);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("31232a10-516d-43a1-bc02-229dab4e6a6c", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "55ba47e1-2157-4400-ac2f-a8ecd791493b")]
        [InlineData("31232a10-516d-43a1-bc02-229dab4e6a6c", "55ba47e1-2157-4400-ac2f-a8ecd791493b")]
        public async Task UpdateCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Arrange
            CollectableUpdateDto collectable = new CollectableUpdateDto
            {
                CollectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"),
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            //Act
            var response = await _controller.UpdateCollectable(new Guid(collectionId), new Guid(id), collectable);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollectable_ReturnsNoContentResponse_GivenValidCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableUpdateDto collectable = new CollectableUpdateDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            //Act
            var response = await _controller.UpdateCollectable(collectionId, id, collectable);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCollectable_UpdatesExistingCollectable_GivenValidCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            CollectableUpdateDto collectable = new CollectableUpdateDto
            {
                CollectableId = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3")
            };

            //Act
            var response = await _controller.UpdateCollectable(collectionId, id, collectable);

            //Assert
            Assert.NotNull(await _unitOfWork.CollectableRepository.GetById(collectionId, id));
            Assert.Equal("Mexico", (await _unitOfWork.CollectableRepository.GetById(collectionId, id))
                .Collectable.Country.Name);
        }

        [Fact]
        public async Task PartiallyUpdateCollectable_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCollectable(Guid.Empty, Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("8185e242-e8fd-4776-a618-17717702a3ed", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "31a58da0-6e0a-4d33-b54d-8c33759c0473")]
        [InlineData("8185e242-e8fd-4776-a618-17717702a3ed", "31a58da0-6e0a-4d33-b54d-8c33759c0473")]
        public async Task PartiallyUpdateCollectable_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Arrange
            JsonPatchDocument<CollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectableUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCollectable(new Guid(collectionId), new Guid(id), patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectable_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectable()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectableUpdateDto>();
            _controller.ModelState.AddModelError("Condition", "Required");

            //Act
            var response = await _controller.PartiallyUpdateCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectable_ReturnsBadRequestResponse_GivenInvalidCollectionId()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectionId, new Guid("10ab82c7-b111-4c09-8793-e2d2d08a8178"));

            //Act
            var response = await _controller.PartiallyUpdateCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("28ee2c6b-dab3-4616-a92f-cc7138228cb8")]
        [InlineData("54826cab-0395-4304-8c2f-6c3bdc82237f")]
        public async Task PartiallyUpdateCollectable_ReturnsBadRequestResponse_GivenInvalidCollectableItemId(string collectableItemId)
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid(collectableItemId));

            //Act
            var response = await _controller.PartiallyUpdateCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectable_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));

            //Act
            var response = await _controller.PartiallyUpdateCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectable_UpdatesExistingCollectable_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");
            JsonPatchDocument<CollectableUpdateDto> patchDoc = new JsonPatchDocument<CollectableUpdateDto>();
            patchDoc.Replace(c => c.CollectableId, new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"));

            //Act
            var response = await _controller.PartiallyUpdateCollectable(collectionId, id, patchDoc);

            //Assert
            Assert.NotNull(await _unitOfWork.CollectableRepository.GetById(collectionId, id));
            Assert.Equal("Mexico", (await _unitOfWork.CollectableRepository.GetById(collectionId, id))
                .Collectable.Country.Name);
        }

        [Theory]
        [InlineData("23fabd7c-7dd0-4db4-a517-1db5351826ab", "355e785b-dd47-4fb7-b112-1fb34d189569")]
        [InlineData("46df9402-62e1-4ff6-9cb0-0955957ec789", "4dc6a62e-c239-49db-8976-548015e1a7fd")]
        [InlineData("23fabd7c-7dd0-4db4-a517-1db5351826ab", "4dc6a62e-c239-49db-8976-548015e1a7fd")]
        public async Task DeleteBanknote_ReturnsNotFoundResponse_GivenInvalidIds(string collectionId, string id)
        {
            //Act
            var response = await _controller.DeleteCollectable(new Guid(collectionId), new Guid(id));

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCollectable_ReturnsNoContentResponse_GivenValidIds()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            var response = await _controller.DeleteCollectable(collectionId, id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteBanknote_RemovesBanknoteFromDatabase()
        {
            //Arrange
            Guid id = new Guid("355e785b-dd47-4fb7-b112-1fb34d189569");
            Guid collectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789");

            //Act
            await _controller.DeleteCollectable(collectionId, id);

            //Assert
            Assert.Single(await _unitOfWork.CollectableRepository.Get(collectionId, resourceParameters));
            Assert.Null(await _unitOfWork.CollectableRepository.GetById(collectionId, id));
        }*/
    }
}