using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Recollectable.API.Controllers;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Controllers
{
    public class BanknoteControllerTests : RecollectableTestBase
    {
        private readonly BanknotesController _controller;
        private readonly CurrenciesResourceParameters resourceParameters;

        public BanknoteControllerTests()
        {
            _controller = new BanknotesController(_unitOfWork, _controllerService);
            resourceParameters = new CurrenciesResourceParameters();

            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<RecollectableMappingProfile>();
            });
        }

        [Fact]
        public void GetBanknotes_ReturnsOkResult_GivenNoMediaType()
        {
            //Act
            var response = _controller.GetBanknotes(resourceParameters, null);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void GetBanknotes_ReturnsAllBanknotes_GivenNoMediaType()
        {
            //Act
            var response = _controller.GetBanknotes(resourceParameters, null) as OkObjectResult;

            //Assert
            var items = Assert.IsType<List<BanknoteDto>>(response.Value);
            Assert.Equal(6, items.Count);
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