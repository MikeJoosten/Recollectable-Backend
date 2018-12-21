using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class BanknoteServiceTests : RecollectableTestBase
    {
        private readonly IBanknoteService _banknoteService;
        private CurrenciesResourceParameters resourceParameters;

        public BanknoteServiceTests()
        {
            _banknoteService = new BanknoteService(_unitOfWork);
            resourceParameters = new CurrenciesResourceParameters();
        }

        [Fact]
        public async Task FindBanknotes_ReturnsAllBanknotes()
        {
            //Act
            var result = await _banknoteService.FindBanknotes(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task FindBanknotes_OrdersBanknotesByValue()
        {
            //Act
            var result = await _banknoteService.FindBanknotes(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Fact]
        public async Task FindBanknoteById_ReturnsBanknote_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89");

            //Act
            var result = await _banknoteService.FindBanknoteById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Pounds", result.Type);
        }

        [Fact]
        public async Task FindBanknoteById_ReturnsNull_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7");

            //Act
            var result = await _banknoteService.FindBanknoteById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBanknote_CreatesNewBanknote()
        {
            //Arrange
            Guid id = new Guid("86dbe5cf-df75-41a5-af56-6e2f2de181a4");
            Banknote newBanknote = new Banknote
            {
                Id = id,
                Type = "Euros",
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                CollectorValueId = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d")
            };

            //Act
            await _banknoteService.CreateBanknote(newBanknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _banknoteService.FindBanknotes(resourceParameters)).Count());
            Assert.Equal("Euros", (await _banknoteService.FindBanknoteById(id)).Type);
        }

        [Fact]
        public async Task UpdateBanknote_UpdatesExistingBanknote()
        {
            //Arrange
            Guid id = new Guid("48d9049b-04f0-4c24-a1c3-c3668878013e");
            Banknote updatedBanknote = await _banknoteService.FindBanknoteById(id);
            updatedBanknote.Type = "Euros";

            //Act
            _banknoteService.UpdateBanknote(updatedBanknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _banknoteService.FindBanknotes(resourceParameters)).Count());
            Assert.Equal("Euros", (await _banknoteService.FindBanknoteById(id)).Type);
        }

        [Fact]
        public async Task RemoveBanknote_RemovesBanknoteFromDatabase()
        {
            //Arrange
            Guid id = new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c");
            Banknote banknote = await _banknoteService.FindBanknoteById(id);

            //Act
            _banknoteService.RemoveBanknote(banknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _banknoteService.FindBanknotes(resourceParameters)).Count());
            Assert.Null(await _banknoteService.FindBanknoteById(id));
        }

        [Fact]
        public async Task BanknoteExists_ReturnsTrue_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89");

            //Act
            var result = await _banknoteService.BanknoteExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task BanknoteExists_ReturnsFalse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7");

            //Act
            var result = await _banknoteService.BanknoteExists(id);

            //Assert
            Assert.False(result);
        }
    }
}