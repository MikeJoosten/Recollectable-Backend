using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Models.Locations;
using Recollectable.Core.Entities.Locations;
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
    public class CountriesControllerTests : RecollectableTestBase
    {
        private readonly CountriesController _controller;
        private readonly Mock<ICountryService> _mockCountryService;
        private readonly CountriesResourceParameters resourceParameters;
        private readonly CountryTestBuilder _builder;

        public CountriesControllerTests()
        {
            _mockCountryService = new Mock<ICountryService>();
            _mockCountryService.Setup(c => c.Save()).ReturnsAsync(true);

            _controller = new CountriesController(_mockCountryService.Object, _mapper);
            SetupTestController(_controller);

            _builder = new CountryTestBuilder();
            resourceParameters = new CountriesResourceParameters();
        }

        [Fact]
        public async Task GetCountries_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            resourceParameters.OrderBy = "Invalid";

            //Act
            var response = await _controller.GetCountries(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCountries_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            resourceParameters.Fields = "Invalid";

            //Act
            var response = await _controller.GetCountries(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCountries_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            var countries = _builder.Build(2);
            var pagedList = PagedList<Country>.Create(countries, 
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCountryService
                .Setup(c => c.FindCountries(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCountries_ReturnsAllCountries_GivenNoMediaType()
        {
            //Arrange
            var countries = _builder.Build(2);
            var pagedList = PagedList<Country>.Create(countries, 
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCountryService
                .Setup(c => c.FindCountries(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCountries(resourceParameters, null) as OkObjectResult;
            var result = response.Value as List<CountryDto>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCountries_ReturnsAllCountries_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            var countries = _builder.Build(2);
            var pagedList = PagedList<Country>.Create(countries,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCountryService
                .Setup(c => c.FindCountries(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCountries_ReturnsAllCountries_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var countries = _builder.Build(2);
            var pagedList = PagedList<Country>.Create(countries,
                resourceParameters.Page, resourceParameters.PageSize);

            _mockCountryService
                .Setup(c => c.FindCountries(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCountries_ReturnsCountries_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            var countries = _builder.Build(4);
            var pagedList = PagedList<Country>.Create(countries, 1, 2);

            _mockCountryService
                .Setup(c => c.FindCountries(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCountries_ReturnsCountries_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var countries = _builder.Build(4);
            var pagedList = PagedList<Country>.Create(countries, 1, 2);

            _mockCountryService
                .Setup(c => c.FindCountries(resourceParameters))
                .ReturnsAsync(pagedList);

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var result = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetCountry_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            string fields = "Invalid";

            //Act
            var response = await _controller.GetCountry(Guid.Empty, fields, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task GetCountry_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Act
            var response = await _controller.GetCountry(Guid.Empty, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public async Task GetCountry_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var country = _builder.Build();

            _mockCountryService
                .Setup(c => c.FindCountryById(id))
                .ReturnsAsync(country);

            //Act
            var response = await _controller.GetCountry(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCountry_ReturnsCountry_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var country = _builder.WithId(id).WithName("Japan").Build();

            _mockCountryService
                .Setup(c => c.FindCountryById(id))
                .ReturnsAsync(country);

            //Act
            var response = await _controller.GetCountry(id, null, null) as OkObjectResult;
            var result = response.Value as CountryDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public async Task GetCountry_ReturnsCountry_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var country = _builder.WithId(id).WithName("Japan").Build();

            _mockCountryService
                .Setup(c => c.FindCountryById(id))
                .ReturnsAsync(country);

            //Act
            var response = await _controller.GetCountry(id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public async Task GetCountry_ReturnsCountry_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var country = _builder.WithId(id).WithName("Japan").Build();

            _mockCountryService
                .Setup(c => c.FindCountryById(id))
                .ReturnsAsync(country);

            //Act
            var response = await _controller.GetCountry(id, null, mediaType) as OkObjectResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public async Task CreateCountry_ReturnsBadRequestResponse_GivenNoCountry()
        {
            //Act
            var response = await _controller.CreateCountry(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task CreateCountry_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCountry()
        {
            //Arrange
            var country = _builder.BuildCreationDto();
            _controller.ModelState.AddModelError("Name", "Required");

            //Act
            var response = await _controller.CreateCountry(country, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public async Task CreateCountry_ReturnsCreatedResponse_GivenValidCountry(string mediaType)
        {
            //Arrange
            var country = _builder.WithName("China").BuildCreationDto();

            //Act
            var response = await _controller.CreateCountry(country, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCountry_CreatesNewCountry_GivenAnyMediaTypeAndValidCountry()
        {
            //Arrange
            var country = _builder.WithName("China").BuildCreationDto();

            //Act
            var response = await _controller.CreateCountry(country, null) as CreatedAtRouteResult;
            var result = response.Value as CountryDto;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("China", result.Name);
        }

        [Fact]
        public async Task CreateCountry_CreatesNewCountry_GivenHateoasMediaTypeAndValidCountry()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            var country = _builder.WithName("China").BuildCreationDto();

            //Act
            var response = await _controller.CreateCountry(country, mediaType) as CreatedAtRouteResult;
            dynamic result = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(result);
            Assert.Equal("China", result.Name);
        }

        [Fact]
        public async Task BlockCountryCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            _mockCountryService.Setup(c => c.CountryExists(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            var response = await _controller.BlockCountryCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            _mockCountryService.Verify(c => c.CountryExists(id));
        }

        [Fact]
        public async Task BlockCountryCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Act
            var response = await _controller.BlockCountryCreation(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCountry_ReturnsBadRequestResponse_GivenNoCountry()
        {
            //Act
            var response = await _controller.UpdateCountry(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task UpdateCountry_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCountry()
        {
            //Arrange
            var country = _builder.BuildUpdateDto();
            _controller.ModelState.AddModelError("Name", "Required");

            //Act
            var response = await _controller.UpdateCountry(Guid.Empty, country);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCountry_ReturnsNotFoundResponse_GivenInvalidCountryId()
        {
            //Arrange
            var country = _builder.WithName("China").BuildUpdateDto();

            //Act
            var response = await _controller.UpdateCountry(Guid.Empty, country);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCountry_ReturnsNoContentResponse_GivenValidCountry()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var country = _builder.WithName("China").BuildUpdateDto();
            var retrievedCountry = _builder.Build();

            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(retrievedCountry);

            //Act
            var response = await _controller.UpdateCountry(id, country);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UpdateCountry_UpdatesExistingCountry_GivenValidCountry()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            var country = _builder.WithName("China").BuildUpdateDto();
            var retrievedCountry = _builder.Build();

            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(retrievedCountry);
            _mockCountryService.Setup(c => c.UpdateCountry(It.IsAny<Country>()));

            //Act
            var response = await _controller.UpdateCountry(id, country);

            //Assert
            _mockCountryService.Verify(c => c.UpdateCountry(retrievedCountry));
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = await _controller.PartiallyUpdateCountry(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsNotFoundResponse_GivenInvalidCountryId()
        {
            //Arrange
            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCountry(Guid.Empty, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsUnprocessableEntityObjectResponse_GivenEqualDescriptionAndName()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            var country = _builder.Build();
            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(country);

            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();
            patchDoc.Replace(c => c.Name, "China");
            patchDoc.Replace(c => c.Description, "China");

            //Act
            var response = await _controller.PartiallyUpdateCountry(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCountry()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            var country = _builder.Build();
            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(country);

            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();
            _controller.ModelState.AddModelError("Name", "Required");

            //Act
            var response = await _controller.PartiallyUpdateCountry(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsNoContentResponse_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            var country = _builder.Build();
            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(country);

            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();
            patchDoc.Replace(c => c.Name, "China");

            //Act
            var response = await _controller.PartiallyUpdateCountry(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_UpdatesExistingCountry_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            var country = _builder.WithId(id).Build();
            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(country);
            _mockCountryService.Setup(c => c.UpdateCountry(It.IsAny<Country>()));

            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();
            patchDoc.Replace(c => c.Name, "China");

            //Act
            var response = await _controller.PartiallyUpdateCountry(id, patchDoc);

            //Assert
            _mockCountryService.Verify(c => c.UpdateCountry(country));
        }

        [Fact]
        public async Task DeleteCountry_ReturnsNotFoundResponse_GivenInvalidCountryId()
        {
            //Act
            var response = await _controller.DeleteCountry(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCountry_ReturnsNoContentResponse_GivenValidCountryId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            var country = _builder.Build();
            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(country);

            //Act
            var response = await _controller.DeleteCountry(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteCountry_RemovesCountryFromDatabase()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            var country = _builder.WithId(id).Build();
            _mockCountryService.Setup(c => c.FindCountryById(id)).ReturnsAsync(country);
            _mockCountryService.Setup(c => c.RemoveCountry(It.IsAny<Country>()));

            //Act
            await _controller.DeleteCountry(id);

            //Assert
            _mockCountryService.Verify(c => c.RemoveCountry(country));
        }
    }
}