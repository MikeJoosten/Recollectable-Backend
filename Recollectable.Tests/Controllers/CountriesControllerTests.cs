using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Locations;
using Recollectable.Core.Shared.Entities;
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
        private readonly CountriesResourceParameters resourceParameters;

        /*public CountriesControllerTests()
        {
            _controller = new CountriesController(_unitOfWork, _typeHelperService,
                _propertyMappingService, _mapper);

            resourceParameters = new CountriesResourceParameters();
            SetupTestController<CountryDto, Country>(_controller);
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
            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetCountries_ReturnsAllCountries_GivenNoMediaType()
        {
            //Act
            var response = await _controller.GetCountries(resourceParameters, null) as OkObjectResult;
            var countries = response.Value as List<CountryDto>;

            //Assert
            Assert.NotNull(countries);
            Assert.Equal(6, countries.Count);
        }

        [Fact]
        public async Task GetCountries_ReturnsAllCountries_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var countries = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(countries);
            Assert.Equal(6, countries.Count);
        }

        [Fact]
        public async Task GetCountries_ReturnsAllCountries_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public async Task GetCountries_ReturnsCountries_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var countries = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(countries);
            Assert.Equal(2, countries.Count);
        }

        [Fact]
        public async Task GetCountries_ReturnsCountries_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = await _controller.GetCountries(resourceParameters, mediaType) as OkObjectResult;
            var countries = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(countries);
            Assert.Equal(2, countries.Value.Count());
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
            //Arrange
            Guid id = new Guid("a1f10f69-61a0-4823-bc55-96e2f04b2e50");

            //Act
            var response = await _controller.GetCountry(id, null, null);

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

            //Act
            var response = await _controller.GetCountry(id, null, null) as OkObjectResult;
            var country = response.Value as CountryDto;

            //Assert
            Assert.NotNull(country);
            Assert.Equal(id, country.Id);
            Assert.Equal("Japan", country.Name);
        }

        [Fact]
        public async Task GetCountry_ReturnsCountry_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var response = await _controller.GetCountry(id, null, mediaType) as OkObjectResult;
            dynamic country = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(country);
            Assert.Equal(id, country.Id);
            Assert.Equal("Japan", country.Name);
        }

        [Fact]
        public async Task GetCountry_ReturnsCountry_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var response = await _controller.GetCountry(id, null, mediaType) as OkObjectResult;
            dynamic country = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(country);
            Assert.Equal(id, country.Id);
            Assert.Equal("Japan", country.Name);
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
        public async Task CreateCountry_ReturnsUnprocessableEntityObjectResponse_GivenInvalidCoin()
        {
            //Arrange
            CountryCreationDto country = new CountryCreationDto();
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
            CountryCreationDto country = new CountryCreationDto
            {
                Name = "China"
            };

            //Act
            var response = await _controller.CreateCountry(country, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public async Task CreateCountry_CreatesNewCountry_GivenAnyMediaTypeAndValidCountry()
        {
            //Arrange
            CountryCreationDto country = new CountryCreationDto
            {
                Name = "China"
            };

            //Act
            var response = await _controller.CreateCountry(country, null) as CreatedAtRouteResult;
            var returnedCountry = response.Value as CountryDto;

            //Assert
            Assert.NotNull(returnedCountry);
            Assert.Equal("China", returnedCountry.Name);
        }

        [Fact]
        public async Task CreateCountry_CreatesNewCountry_GivenHateoasMediaTypeAndValidCountry()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            CountryCreationDto country = new CountryCreationDto
            {
                Name = "China"
            };

            //Act
            var response = await _controller.CreateCountry(country, mediaType) as CreatedAtRouteResult;
            dynamic returnedCountry = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedCountry);
            Assert.Equal("China", returnedCountry.Name);
        }

        [Fact]
        public async Task BlockCountryCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var response = await _controller.BlockCountryCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BlockCountryCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("d60ce4c3-7e55-43f0-a0a5-b7cef8e020f8");

            //Act
            var response = await _controller.BlockCountryCreation(id);

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
            CountryUpdateDto country = new CountryUpdateDto();
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
            Guid id = new Guid("db069604-0ea6-4a05-b7d4-1e6b8470c748");
            CountryUpdateDto country = new CountryUpdateDto
            {
                Name = "China"
            };

            //Act
            var response = await _controller.UpdateCountry(id, country);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task UpdateCountry_ReturnsNoContentResponse_GivenValidCountry()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
            CountryUpdateDto country = new CountryUpdateDto
            {
                Name = "China"
            };

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
            CountryUpdateDto country = new CountryUpdateDto
            {
                Name = "China"
            };

            //Act
            var response = await _controller.UpdateCountry(id, country);

            //Assert
            Assert.NotNull(await _unitOfWork.CountryRepository.GetById(id));
            Assert.Equal("China", (await _unitOfWork.CountryRepository.GetById(id)).Name);
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
            Guid id = new Guid("2368b256-5f1f-49f5-8f01-836df3725a76");
            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();

            //Act
            var response = await _controller.PartiallyUpdateCountry(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task PartiallyUpdateCountry_ReturnsUnprocessableEntityObjectResponse_GivenEqualDescriptionAndName()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");
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
            JsonPatchDocument<CountryUpdateDto> patchDoc = new JsonPatchDocument<CountryUpdateDto>();
            patchDoc.Replace(c => c.Name, "China");

            //Act
            var response = await _controller.PartiallyUpdateCountry(id, patchDoc);

            //Assert
            Assert.NotNull(await _unitOfWork.CountryRepository.GetById(id));
            Assert.Equal("China", (await _unitOfWork.CountryRepository.GetById(id)).Name);
        }

        [Fact]
        public async Task DeleteCountry_ReturnsNotFoundResponse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("08786b17-8443-4873-98b3-8d73cf400fba");

            //Act
            var response = await _controller.DeleteCountry(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task DeleteCountry_ReturnsNoContentResponse_GivenValidCountryId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

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

            //Act
            await _controller.DeleteCountry(id);

            //Assert
            Assert.Equal(5, (await _unitOfWork.CountryRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.CountryRepository.GetById(id));
        }*/
    }
}