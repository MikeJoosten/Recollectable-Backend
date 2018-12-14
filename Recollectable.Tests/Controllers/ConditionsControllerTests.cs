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

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
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
        public async Task GetConditions_ReturnsAllConditions_GivenNoMediaType()
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
            var result = response.Value as List<ConditionDto>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetConditions_ReturnsAllConditions_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            var conditions = _builder.Build(2);
            var pagedList = PagedList<Condition>.Create(conditions,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, mediaType) as OkObjectResult;
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
        public async Task GetConditions_ReturnsConditions_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            var conditions = _builder.Build(4);
            var pagedList = PagedList<Condition>.Create(conditions, 1, 2);

            _mockConditionService
                .Setup(c => c.FindConditions(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetConditions(resourceParameters, mediaType) as OkObjectResult;
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

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
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
            var response = await _controller.GetCondition(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCondition_ReturnsCondition_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("64dc0403-db60-479a-bce4-8662e3a16e55");
            var condition = _builder.WithId(id).WithGrade("XF48").Build();

            _mockConditionService
                .Setup(c => c.FindConditionById(id))
                .ReturnsAsync(condition);

            //Act
            var response = await _controller.GetCondition(id, null, null) as OkObjectResult;
            var result = response.Value as ConditionDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("XF48", result.Grade);
        }

        [Fact]
        public async Task GetCondition_ReturnsCondition_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var condition = _builder.WithId(id).WithGrade("XF48").Build();

            _mockConditionService
                .Setup(c => c.FindConditionById(id))
                .ReturnsAsync(condition);

            //Act
            var response = await _controller.GetCondition(id, null, mediaType) as OkObjectResult;
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
            var response = await _controller.GetCondition(id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("XF48", result.Grade);
        }

        
    }
}
