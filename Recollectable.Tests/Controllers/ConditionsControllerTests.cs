using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Models.Collections;
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
    public class ConditionsControllerTests : RecollectableTestBase
    {
        private readonly ConditionsController _controller;
        private readonly Mock<IConditionService> _mockConditionService;
        private readonly ConditionsResourceParameters resourceParameters;
        private readonly ConditionTestBuilder _builder;

        public ConditionsControllerTests()
        {
            _mockConditionService = new Mock<IConditionService>();
            _mockConditionService.Setup(c => c.Save()).ReturnsAsync(true);

            _controller = new ConditionsController(_mockConditionService.Object, _mapper);
            SetupTestController(_controller);

            _builder = new ConditionTestBuilder();
            resourceParameters = new ConditionsResourceParameters();
            resourceParameters.Fields = "Id, Grade";
        }

        [Fact]
        public async Task GetConditions_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetConditions(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetConditions_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetConditions(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetConditions_ReturnsBadRequestObjectResponse_GivenFieldParameterWithNoId()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var conditions = _builder.Build(2);
            var pagedList = PagedList<Condition>.Create(conditions,
                resourceParameters.Page, resourceParameters.PageSize);
            resourceParameters.Fields = "Grade";

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, mediaType);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetConditions_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            var conditions = _builder.Build(2);
            var pagedList = PagedList<Condition>.Create(conditions,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetConditions_ReturnsAllConditions_GivenAnyMediaType()
        {
            //Arrange
            var conditions = _builder.Build(2);
            var pagedList = PagedList<Condition>.Create(conditions,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetConditions_ReturnsAllConditions_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var conditions = _builder.Build(2);
            var pagedList = PagedList<Condition>.Create(conditions,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetConditions_ReturnsConditions_GivenAnyMediaTypeAndPagingParameters()
        {
            //Arrange
            var conditions = _builder.Build(4);
            var pagedList = PagedList<Condition>.Create(conditions, 1, 2);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetConditions_ReturnsConditions_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var conditions = _builder.Build(4);
            var pagedList = PagedList<Condition>.Create(conditions, 1, 2);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCondition_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCondition(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCondition_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Act
            var response = await _controller.GetCondition(Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task GetCondition_ReturnsBadRequestObjectResponse_GivenFieldParameterWithNoId()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var condition = _builder.WithId(id).WithGrade("XF48").Build();
            resourceParameters.Fields = "Grade";

            _mockConditionService
                .Setup(c => c.FindConditionById(id))
                .ReturnsAsync(condition);

            //Act
            var response = await _controller.GetCondition(id, resourceParameters.Fields, mediaType);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task GetCondition_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            var condition = _builder.Build();

            _mockConditionService
                .Setup(c => c.FindConditionById(id))
                .ReturnsAsync(condition);

            //Act
            var response = await _controller.GetCondition(id, resourceParameters.Fields, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCondition_ReturnsCondition_GivenAnyMediaType()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var condition = _builder.WithId(id).WithGrade("XF48").Build();

            _mockConditionService
                .Setup(c => c.FindConditionById(id))
                .ReturnsAsync(condition);

            //Act
            var response = await _controller.GetCondition(id, null, null) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("XF48", result.Grade);
        }

        [Fact]
        public async Task GetCondition_ReturnsCondition_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var condition = _builder.WithId(id).WithGrade("XF48").Build();

            _mockConditionService
                .Setup(c => c.FindConditionById(id))
                .ReturnsAsync(condition);

            //Act
            var response = await _controller.GetCondition(id, resourceParameters.Fields, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("XF48", result.Grade);
        }

        [Fact]
        public async Task CreateCondition_ReturnsBadRequestResponse_GivenNoCondition()
        {
            //Act
            var response = await _controller.CreateCondition(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCondition_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCondition()
        {
            //Arrange
            var condition = _builder.BuildCreationDto();
            _controller.ModelState.AddModelError("Name", "Required");

            //Act
            var response = await _controller.CreateCondition(condition, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCondition_ReturnsCreatedResponse_GivenValidCondition(string mediaType)
        {
            //Arrange
            var condition = _builder.WithGrade("F12").BuildCreationDto();

            //Act
            var response = await _controller.CreateCondition(condition, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCondition_CreatesNewCondition_GivenAnyMediaTypeAndValidCondition()
        {
            //Arrange
            var condition = _builder.WithGrade("F12").BuildCreationDto();

            //Act
            var response = await _controller.CreateCondition(condition, null) as CreatedAtRouteResult;
            var result = response.Value as ConditionDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("F12", result.Grade);
        }

        [Fact]
        public async Task CreateCondition_CreatesNewCondition_GivenHateoasMediaTypeAndValidCondition()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var condition = _builder.WithGrade("F12").BuildCreationDto();

            //Act
            var response = await _controller.CreateCondition(condition, mediaType) as CreatedAtRouteResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("F12", result.Grade);
        }

        [Fact]
        public async Task BlockConditionCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            _mockConditionService.Setup(c => c.ConditionExists(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            var response = await _controller.BlockConditionCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            _mockConditionService.Verify(c => c.ConditionExists(id));
        }

        [Fact]
        public async Task BlockConditionCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Act
            var response = await _controller.BlockConditionCreation(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCondition_ReturnsBadRequestResponse_GivenNoCondition()
        {
            //Act
            var response = await _controller.UpdateCondition(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCondition_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCondition()
        {
            //Arrange
            var condition = _builder.BuildUpdateDto();
            _controller.ModelState.AddModelError("Grade", "Required");

            //Act
            var response = await _controller.UpdateCondition(Guid.Empty, condition);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCondition_ReturnsNotFoundResponse_GivenInvalidConditionId()
        {
            //Arrange
            var condition = _builder.WithGrade("F12").BuildUpdateDto();

            //Act
            var response = await _controller.UpdateCondition(Guid.Empty, condition);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCondition_ReturnsNoContentResponse_GivenValidCondition()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            var condition = _builder.WithGrade("F12").BuildUpdateDto();
            var retrievedCondition = _builder.Build();

            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(retrievedCondition);

            //Act
            var response = await _controller.UpdateCondition(id, condition);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCondition_UpdatesExistingCountry_GivenValidCondition()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            var condition = _builder.WithGrade("F12").BuildUpdateDto();
            var retrievedCondition = _builder.Build();

            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(retrievedCondition);
            _mockConditionService.Setup(c => c.UpdateCondition(It.IsAny<Condition>()));

            //Act
            var response = await _controller.UpdateCondition(id, condition);

            //Assert
            _mockConditionService.Verify(c => c.UpdateCondition(retrievedCondition));
        }

        [Fact]
        public async Task PartiallyUpdateCondition_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCondition(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCondition_ReturnsNotFoundResponse_GivenInvalidConditionId()
        {
            //Arrange
            JsonPatchDocument<ConditionUpdateDto> patchDoc = new JsonPatchDocument<ConditionUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCondition(Guid.Empty, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCountry()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");

            var condition = _builder.Build();
            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(condition);

            JsonPatchDocument<ConditionUpdateDto> patchDoc = new JsonPatchDocument<ConditionUpdateDto>();
            _controller.ModelState.AddModelError("Grade", "Required");

            //Act
            var response = await _controller.PartiallyUpdateCondition(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCondition_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");

            var condition = _builder.Build();
            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(condition);

            JsonPatchDocument<ConditionUpdateDto> patchDoc = new JsonPatchDocument<ConditionUpdateDto>();
            patchDoc.Replace(c => c.Grade, "F12");

            //Act
            var response = await _controller.PartiallyUpdateCondition(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCondition_UpdatesExistingCondition_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");

            var condition = _builder.WithId(id).Build();
            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(condition);
            _mockConditionService.Setup(c => c.UpdateCondition(It.IsAny<Condition>()));

            JsonPatchDocument<ConditionUpdateDto> patchDoc = new JsonPatchDocument<ConditionUpdateDto>();
            patchDoc.Replace(c => c.Grade, "F12");

            //Act
            var response = await _controller.PartiallyUpdateCondition(id, patchDoc);

            //Assert
            _mockConditionService.Verify(c => c.UpdateCondition(condition));
        }

        [Fact]
        public async Task DeleteCondition_ReturnsNotFoundResponse_GivenInvalidConditionId()
        {
            //Act
            var response = await _controller.DeleteCondition(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCondition_ReturnsNoContentResponse_GivenValidConditionId()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");

            var condition = _builder.Build();
            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(condition);

            //Act
            var response = await _controller.DeleteCondition(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteCondition_RemovesConditionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");

            var condition = _builder.WithId(id).Build();
            _mockConditionService.Setup(c => c.FindConditionById(id)).ReturnsAsync(condition);
            _mockConditionService.Setup(c => c.RemoveCondition(It.IsAny<Condition>()));

            //Act
            await _controller.DeleteCondition(id);

            //Assert
            _mockConditionService.Verify(c => c.RemoveCondition(condition));
        }
    }
}