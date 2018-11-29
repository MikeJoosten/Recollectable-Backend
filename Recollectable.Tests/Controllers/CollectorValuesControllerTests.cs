﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.Collectables;
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
    public class CollectorValuesControllerTests : RecollectableTestBase
    {
        private readonly CollectorValuesController _controller;
        private readonly CollectorValuesResourceParameters resourceParameters;

        /*public CollectorValuesControllerTests()
        {
            _controller = new CollectorValuesController(_unitOfWork, _typeHelperService,
                _propertyMappingService, _mapper);

            resourceParameters = new CollectorValuesResourceParameters();
            SetupTestController<CollectorValueDto, CollectorValue>(_controller);
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectorValues_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsAllCollectorValues_GivenNoMediaType()
        {
            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, null) as OkObjectResult;
            var collectorValues = response.Value as List<CollectorValueDto>;

            //Assert
            Assert.NotNull(collectorValues);
            Assert.Equal(6, collectorValues.Count);
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsAllCollectorValues_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, mediaType) as OkObjectResult;
            var collectorValues = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(collectorValues);
            Assert.Equal(6, collectorValues.Count);
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsAllCollectorValues_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsCollectorValues_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, mediaType) as OkObjectResult;
            var collectorValues = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(collectorValues);
            Assert.Equal(2, collectorValues.Count);
        }

        [Fact]
        public async Task GetCollectorValues_ReturnsCollectorValues_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetCollectorValues(resourceParameters, mediaType) as OkObjectResult;
            var collectorValues = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(collectorValues);
            Assert.Equal(2, collectorValues.Value.Count());
        }

        [Fact]
        public async Task GetCollectorValue_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCollectorValue(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCollectorValue_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("650f3296-894a-486a-b259-aea82a935981");

            //Act
            var response = await _controller.GetCollectorValue(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCollectorValue_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            var response = await _controller.GetCollectorValue(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCollectorValue_ReturnsCollectorValue_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            var response = await _controller.GetCollectorValue(id, null, null) as OkObjectResult;
            var collectorValue = response.Value as CollectorValueDto;

            //Assert
            Assert.NotNull(collectorValue);
            Assert.Equal(id, collectorValue.Id);
            Assert.Equal(15.54, collectorValue.G4);
        }

        [Fact]
        public async Task GetCollectorValue_ReturnsCollectorValue_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            var response = await _controller.GetCollectorValue(id, null, mediaType) as OkObjectResult;
            dynamic collectorValue = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(collectorValue);
            Assert.Equal(id, collectorValue.Id);
            Assert.Equal(15.54, collectorValue.G4);
        }

        [Fact]
        public async Task GetCollectorValue_ReturnsCollectorValue_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            var response = await _controller.GetCollectorValue(id, null, mediaType) as OkObjectResult;
            dynamic collectorValue = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(collectorValue);
            Assert.Equal(id, collectorValue.Id);
            Assert.Equal(15.54, collectorValue.G4);
        }

        [Fact]
        public async Task CreateCollectorValue_ReturnsBadRequestResponse_GivenNoCollection()
        {
            //Act
            var response = await _controller.CreateCollectorValue(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCollectorValue_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectorValue()
        {
            //Arrange
            CollectorValueCreationDto collectorValue = new CollectorValueCreationDto();
            _controller.ModelState.AddModelError("G4", "Required");

            //Act
            var response = await _controller.CreateCollectorValue(collectorValue, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCollectorValue_ReturnsCreatedResponse_GivenValidCollectorValue(string mediaType)
        {
            //Arrange
            CollectorValueCreationDto collectorValue = new CollectorValueCreationDto
            {
                G4 = 18.64
            };

            //Act
            var response = await _controller.CreateCollectorValue(collectorValue, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCollectorValue_CreatesNewCollectorValue_GivenAnyMediaTypeAndValidCollectorValue()
        {
            //Arrange
            CollectorValueCreationDto collectorValue = new CollectorValueCreationDto
            {
                G4 = 18.64
            };

            //Act
            var response = await _controller.CreateCollectorValue(collectorValue, null) as CreatedAtRouteResult;
            var returnedCollectorValue = response.Value as CollectorValueDto;

            //Assert
            Assert.NotNull(returnedCollectorValue);
            Assert.Equal(18.64, returnedCollectorValue.G4);
        }

        [Fact]
        public async Task CreateCollectorValue_CreatesNewCollectorValue_GivenHateoasMediaTypeAndValidCollectorValue()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            CollectorValueCreationDto collectorValue = new CollectorValueCreationDto
            {
                G4 = 18.64
            };

            //Act
            var response = await _controller.CreateCollectorValue(collectorValue, mediaType) as CreatedAtRouteResult;
            dynamic returnedCollectorValue = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCollectorValue);
            Assert.Equal(18.64, returnedCollectorValue.G4);
        }

        [Fact]
        public async Task BlockCountryCollectorValue_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            var response = await _controller.BlockCollectorValueCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BlockCollectorValueCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("3323fa30-f1a9-47d2-8c26-e354b508eba6");

            //Act
            var response = await _controller.BlockCollectorValueCreation(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollectorValue_ReturnsBadRequestResponse_GivenNoCollectorValue()
        {
            //Act
            var response = await _controller.UpdateCollectorValue(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCollectorValue_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectorValue()
        {
            //Arrange
            CollectorValueUpdateDto collectorValue = new CollectorValueUpdateDto();
            _controller.ModelState.AddModelError("G4", "Required");

            //Act
            var response = await _controller.UpdateCollectorValue(Guid.Empty, collectorValue);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCollectorValue_ReturnsNotFoundResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("358e52b6-3ad9-4ada-8f09-04919e30fd65");
            CollectorValueUpdateDto collectorValue = new CollectorValueUpdateDto
            {
                G4 = 18.64
            };

            //Act
            var response = await _controller.UpdateCollectorValue(id, collectorValue);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCollectorValue_ReturnsNoContentResponse_GivenValidCollectorValue()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            CollectorValueUpdateDto collectorValue = new CollectorValueUpdateDto
            {
                G4 = 18.64
            };

            //Act
            var response = await _controller.UpdateCollectorValue(id, collectorValue);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCollectorValue_UpdatesExistingCollectorValue_GivenValidCollectorValue()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            CollectorValueUpdateDto collectorValue = new CollectorValueUpdateDto
            {
                G4 = 18.64
            };

            //Act
            var response = await _controller.UpdateCollectorValue(id, collectorValue);

            //Assert
            Assert.NotNull(await _unitOfWork.CollectorValueRepository.GetById(id));
            Assert.Equal(18.64, (await _unitOfWork.CollectorValueRepository.GetById(id)).G4);
        }

        [Fact]
        public async Task PartiallyUpdateCollectorValue_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCollectorValue(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectorValue_ReturnsNotFoundResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("615ad0ae-0c91-4e39-99e1-c9f74b92a7e8");
            JsonPatchDocument<CollectorValueUpdateDto> patchDoc = new JsonPatchDocument<CollectorValueUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCollectorValue(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectorValue_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCollectorValue()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            JsonPatchDocument<CollectorValueUpdateDto> patchDoc = new JsonPatchDocument<CollectorValueUpdateDto>();
            _controller.ModelState.AddModelError("G4", "Required");

            //Act
            var response = await _controller.PartiallyUpdateCollectorValue(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectorValue_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            JsonPatchDocument<CollectorValueUpdateDto> patchDoc = new JsonPatchDocument<CollectorValueUpdateDto>();
            patchDoc.Replace(c => c.G4, 18.64);

            //Act
            var response = await _controller.PartiallyUpdateCollectorValue(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCollectorValue_UpdatesExistingCollectorValue_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");
            JsonPatchDocument<CollectorValueUpdateDto> patchDoc = new JsonPatchDocument<CollectorValueUpdateDto>();
            patchDoc.Replace(c => c.G4, 18.64);

            //Act
            var response = await _controller.PartiallyUpdateCollectorValue(id, patchDoc);

            //Assert
            Assert.NotNull(await _unitOfWork.CollectorValueRepository.GetById(id));
            Assert.Equal(18.64, (await _unitOfWork.CollectorValueRepository.GetById(id)).G4);
        }

        [Fact]
        public async Task DeleteCollectorValue_ReturnsNotFoundResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("db23a489-535a-49c2-8af3-82490b3e50ef");

            //Act
            var response = await _controller.DeleteCollectorValue(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCollectorValue_ReturnsNoContentResponse_GivenValidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            var response = await _controller.DeleteCollectorValue(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteCollectorValue_RemovesCollectorValueFromDatabase()
        {
            //Arrange
            Guid id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5");

            //Act
            await _controller.DeleteCollectorValue(id);

            //Assert
            Assert.Equal(5, (await _unitOfWork.CollectorValueRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.CollectorValueRepository.GetById(id));
        }*/
    }
}