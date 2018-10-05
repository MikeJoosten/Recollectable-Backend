using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Recollectable.API.Controllers;
using Recollectable.API.Services;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class BanknotesControllerTests : RecollectableTestBase
    {
        private readonly BanknotesController _controller;
        private readonly CurrenciesResourceParameters resourceParameters;

        public BanknotesControllerTests()
        {
            _controller = new BanknotesController(_unitOfWork, _mockControllerService.Object);
            resourceParameters = new CurrenciesResourceParameters();

            SetupTestController<BanknoteDto, Banknote>(_controller, 
                PropertyMappingService._currencyPropertyMapping);
        }

        [Fact]
        public void GetBanknotes_ReturnsBadRequestResponse_GivenInvalidOrderByParameter()
        {
            //Arrange
            _mockPropertyMappingService.Setup(x =>
                x.ValidMappingExistsFor<BanknoteDto, Banknote>(It.IsAny<string>())).Returns(false);

            //Act
            var response = _controller.GetBanknotes(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void GetBanknotes_ReturnsBadRequestResponse_GivenInvalidFieldsParameter()
        {
            //Arrange
            _mockTypeHelperService.Setup(x =>
                x.TypeHasProperties<BanknoteDto>(It.IsAny<string>())).Returns(false);

            //Act
            var response = _controller.GetBanknotes(resourceParameters, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public void GetBanknotes_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Act
            var response = _controller.GetBanknotes(resourceParameters, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetBanknotes_ReturnsAllBanknotes_GivenNoMediaType()
        {
            //Act
            var response = _controller.GetBanknotes(resourceParameters, null) as OkObjectResult;
            var banknotes = response.Value as List<BanknoteDto>;

            //Assert
            Assert.NotNull(banknotes);
            Assert.Equal(6, banknotes.Count);
        }

        [Fact]
        public void GetBanknotes_ReturnsAllBanknotes_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";

            //Act
            var response = _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var banknotes = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(banknotes);
            Assert.Equal(6, banknotes.Count);
        }

        [Fact]
        public void GetBanknotes_ReturnsAllBanknotes_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";

            //Act
            var response = _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var linkedCollection = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(linkedCollection);
            Assert.Equal(6, linkedCollection.Value.Count());
        }

        [Fact]
        public void GetBanknotes_ReturnsBanknotes_GivenJsonMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json";
            resourceParameters.PageSize = 2;

            //Act
            var response = _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var items = response.Value as List<ExpandoObject>;

            //Assert
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public void GetBanknotes_ReturnsBanknotes_GivenHateoasMediaTypeAndPagingParameters()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            resourceParameters.PageSize = 2;

            //Act
            var response = _controller.GetBanknotes(resourceParameters, mediaType) as OkObjectResult;
            var items = response.Value as LinkedCollectionResource;

            //Assert
            Assert.NotNull(items);
            Assert.Equal(2, items.Value.Count());
        }

        [Fact]
        public void GetBanknote_ReturnsNotFoundResponse_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("64c21a8d-048b-4b2a-8b88-2b2d7919c0be");

            //Act
            var response = _controller.GetBanknote(id, null, null);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json")]
        [InlineData("application/json+hateoas")]
        public void GetBanknote_ReturnsOkResponse_GivenAnyMediaType(string mediaType)
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");

            //Act
            var response = _controller.GetBanknote(id, null, mediaType);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetBanknote_ReturnsBanknote_GivenNoMediaType()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = _controller.GetBanknote(id, null, null) as OkObjectResult;
            var banknote = response.Value as BanknoteDto;

            //Assert
            Assert.NotNull(banknote);
            Assert.Equal(id, banknote.Id);
            Assert.Equal("Dollars", banknote.Type);
        }

        [Fact]
        public void GetBanknote_ReturnsBanknote_GivenJsonMediaType()
        {
            //Arrange
            string mediaType = "application/json";
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = _controller.GetBanknote(id, null, mediaType) as OkObjectResult;
            dynamic banknote = response.Value as ExpandoObject;

            //Assert
            Assert.NotNull(banknote);
            Assert.Equal(id, banknote.Id);
            Assert.Equal("Dollars", banknote.Type);
        }

        [Fact]
        public void GetBanknote_ReturnsBanknote_GivenHateoasMediaType()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = _controller.GetBanknote(id, null, mediaType) as OkObjectResult;
            dynamic banknote = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(banknote);
            Assert.Equal(id, banknote.Id);
            Assert.Equal("Dollars", banknote.Type);
        }

        [Fact]
        public void CreateBanknote_ReturnsBadRequestResponse_GivenNoBanknote()
        {
            //Act
            var response = _controller.CreateBanknote(null, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void CreateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = _controller.CreateBanknote(banknote, null);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void CreateBanknote_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                CountryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d")
            };

            //Act
            var response = _controller.CreateBanknote(banknote, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void CreateBanknote_ReturnsBadRequestResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("56b08b23-3ef5-4fa7-a607-4d254733a2e8")
            };

            //Act
            var response = _controller.CreateBanknote(banknote, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("application/json+hateoas")]
        public void CreateBanknote_ReturnsCreatedResponse_GivenValidBanknote(string mediaType)
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.CreateBanknote(banknote, mediaType);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(response);
        }

        [Fact]
        public void CreateBanknote_CreatesNewBanknote_GivenJsonMediaTypeAndValidBanknote()
        {
            //Arrange
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.CreateBanknote(banknote, null) as CreatedAtRouteResult;
            var returnedBanknote = response.Value as BanknoteDto;

            //Assert
            Assert.NotNull(returnedBanknote);
            Assert.Equal("Dollar", returnedBanknote.Type);
            Assert.Equal("United States of America", returnedBanknote.Country.Name);
        }

        [Fact]
        public void CreateBanknote_CreatesNewBanknote_GivenHateoasMediaTypeAndValidBanknote()
        {
            //Arrange
            string mediaType = "application/json+hateoas";
            BanknoteCreationDto banknote = new BanknoteCreationDto
            {
                Type = "Dollar",
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.CreateBanknote(banknote, mediaType) as CreatedAtRouteResult;
            dynamic returnedBanknote = response.Value as IDictionary<string, object>;

            //Assert
            Assert.NotNull(returnedBanknote);
            Assert.Equal("Dollar", returnedBanknote.Type);
            Assert.Equal("United States of America", returnedBanknote.Country.Name);
        }

        [Fact]
        public void BlockBanknoteCreation_ReturnsConflictResponse_GivenExistingId()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = _controller.BlockBanknoteCreation(id) as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }

        [Fact]
        public void BlockBanknoteCreation_ReturnsNotFoundResponse_GivenUnexistingId()
        {
            //Arrange
            Guid id = new Guid("03c0e206-8222-4c4a-a8e2-4c2e1cae8ad1");

            //Act
            var response = _controller.BlockBanknoteCreation(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void UpdateBanknote_ReturnsBadRequestResponse_GivenNoBanknote()
        {
            //Act
            var response = _controller.UpdateBanknote(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            BanknoteUpdateDto banknote = new BanknoteUpdateDto();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void UpdateBanknote_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d")
            };

            //Act
            var response = _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateBanknote_ReturnsBadRequestResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("56b08b23-3ef5-4fa7-a607-4d254733a2e8")
            };

            //Act
            var response = _controller.UpdateBanknote(Guid.Empty, banknote);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void UpdateBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("1876de77-90d9-4083-91d2-b6a3e6a1bd1c");
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.UpdateBanknote(id, banknote);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void UpdateBanknote_ReturnsNoContentResponse_GivenValidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.UpdateBanknote(id, banknote);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void UpdateBanknote_UpdatesExistingBanknote_GivenValidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            BanknoteUpdateDto banknote = new BanknoteUpdateDto
            {
                Type = "Euros",
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                CollectorValueId = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5")
            };

            //Act
            var response = _controller.UpdateBanknote(id, banknote);

            //Assert
            Assert.NotNull(_unitOfWork.BanknoteRepository.GetById(id));
            Assert.Equal("Euros", _unitOfWork.BanknoteRepository.GetById(id).Type);
            Assert.Equal("France", _unitOfWork.BanknoteRepository.GetById(id).Country.Name);
        }

        [Fact]
        public void PartiallyUpdateBanknote_ReturnsBadRequestResponse_GivenNoPatchDocument()
        {
            //Act
            var response = _controller.PartiallyUpdateBanknote(Guid.Empty, null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("1876de77-90d9-4083-91d2-b6a3e6a1bd1c");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();

            //Act
            var response = _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void PartiallyUpdateBanknote_ReturnsUnprocessableEntityObjectResponse_GivenInvalidBanknote()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            _controller.ModelState.AddModelError("Type", "Required");

            //Act
            var response = _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(response);
        }

        [Fact]
        public void PartiallyUpdateBanknote_ReturnsBadRequestResponse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(e => e.CountryId, new Guid("e4d31596-b6e0-4ac6-9c18-9bfe5102780d"));

            //Act
            var response = _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateBanknote_ReturnsBadRequestResponse_GivenInvalidCollectorValueId()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(e => e.CollectorValueId, new Guid("56b08b23-3ef5-4fa7-a607-4d254733a2e8"));

            //Act
            var response = _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public void PartiallyUpdateBanknote_ReturnsNoContent_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();

            //Act
            var response = _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void PartiallyUpdateBanknote_UpdatesExistingBanknote_GivenValidPatchDocument()
        {
            //Arrange
            Guid id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4");
            JsonPatchDocument<BanknoteUpdateDto> patchDoc = new JsonPatchDocument<BanknoteUpdateDto>();
            patchDoc.Replace(e => e.Type, "Euros");
            patchDoc.Replace(e => e.CountryId, new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"));

            //Act
            var response = _controller.PartiallyUpdateBanknote(id, patchDoc);

            //Assert
            Assert.NotNull(_unitOfWork.BanknoteRepository.GetById(id));
            Assert.Equal("Euros", _unitOfWork.BanknoteRepository.GetById(id).Type);
            Assert.Equal("France", _unitOfWork.BanknoteRepository.GetById(id).Country.Name);
        }

        [Fact]
        public void DeleteBanknote_ReturnsNoContentResponse_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            var response = _controller.DeleteBanknote(id);

            //Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public void DeleteBanknote_ReturnsNotFoundResponse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("2d11b36a-9f6b-42f9-a00a-6e07b8a30f0e");

            //Act
            var response = _controller.DeleteBanknote(id);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void DeleteBanknote_RemovesBanknoteFromDatabase()
        {
            //Arrange
            Guid id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f");

            //Act
            _controller.DeleteBanknote(id);

            //Assert
            Assert.Equal(5, _unitOfWork.BanknoteRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.BanknoteRepository.GetById(id));
        }
    }
}