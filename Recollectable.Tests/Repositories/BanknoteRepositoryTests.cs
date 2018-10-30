using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class BanknoteRepositoryTests : RecollectableTestBase
    {
        private CurrenciesResourceParameters resourceParameters;

        public BanknoteRepositoryTests()
        {
            resourceParameters = new CurrenciesResourceParameters();
        }

        [Fact]
        public async Task Get_ReturnsAllBanknotes()
        {
            //Act
            var result = await _unitOfWork.BanknoteRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task Get_OrdersCollectionsByCountry()
        {
            //Act
            var result = await _unitOfWork.BanknoteRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Fact]
        public async Task GetById_ReturnsBanknote_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89");

            //Act
            var result = await _unitOfWork.BanknoteRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Pounds", result.Type);
        }

        [Fact]
        public async Task GetById_ReturnsNull_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7");

            //Act
            var result = await _unitOfWork.BanknoteRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_AddsNewBanknote()
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
            _unitOfWork.BanknoteRepository.Add(newBanknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.BanknoteRepository.Get(resourceParameters)).Count());
            Assert.Equal("Euros", (await _unitOfWork.BanknoteRepository.GetById(id)).Type);
        }

        [Fact]
        public async Task Update_UpdatesExistingBanknote()
        {
            //Arrange
            Guid id = new Guid("48d9049b-04f0-4c24-a1c3-c3668878013e");
            Banknote updatedBanknote = await _unitOfWork.BanknoteRepository.GetById(id);
            updatedBanknote.Type = "Euros";

            //Act
            _unitOfWork.BanknoteRepository.Update(updatedBanknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _unitOfWork.BanknoteRepository.Get(resourceParameters)).Count());
            Assert.Equal("Euros", (await _unitOfWork.BanknoteRepository.GetById(id)).Type);
        }

        [Fact]
        public async Task Delete_RemovesBanknoteFromDatabase()
        {
            //Arrange
            Guid id = new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c");
            Banknote banknote = await _unitOfWork.BanknoteRepository.GetById(id);

            //Act
            _unitOfWork.BanknoteRepository.Delete(banknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.BanknoteRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.BanknoteRepository.GetById(id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89");

            //Act
            var result = await _unitOfWork.BanknoteRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidBanknoteId()
        {
            //Arrange
            Guid id = new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7");

            //Act
            var result = await _unitOfWork.BanknoteRepository.Exists(id);

            //Assert
            Assert.False(result);
        }
    }
}