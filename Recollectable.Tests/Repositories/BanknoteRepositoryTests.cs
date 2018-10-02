using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
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
        public void Get_ReturnsAllBanknotes()
        {
            var result = _unitOfWork.BanknoteRepository.Get(resourceParameters);

            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersCollectionsByCountry()
        {
            var result = _unitOfWork.BanknoteRepository.Get(resourceParameters);

            Assert.Equal("Canada", result.First().Country.Name);
        }

        [Fact]
        public void GetById_ReturnsBanknote_GivenValidBanknoteId()
        {
            Guid id = new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89");

            var result = _unitOfWork.BanknoteRepository.GetById(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Pounds", result.Type);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidBanknoteId()
        {
            var result = _unitOfWork.BanknoteRepository
                .GetById(new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7"));

            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewBanknote()
        {
            Guid id = new Guid("86dbe5cf-df75-41a5-af56-6e2f2de181a4");
            Banknote newBanknote = new Banknote
            {
                Id = id,
                Type = "Euros",
                CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                CollectorValueId = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d")
            };

            _unitOfWork.BanknoteRepository.Add(newBanknote);
            _unitOfWork.Save();

            Assert.Equal(7, _unitOfWork.BanknoteRepository.Get(resourceParameters).Count());
            Assert.Equal("Euros", _unitOfWork.BanknoteRepository.GetById(id).Type);
        }

        [Fact]
        public void Update_UpdatesExistingBanknote()
        {
            Guid id = new Guid("48d9049b-04f0-4c24-a1c3-c3668878013e");
            Banknote updatedBanknote = _unitOfWork.BanknoteRepository.GetById(id);
            updatedBanknote.Type = "Euros";

            _unitOfWork.BanknoteRepository.Update(updatedBanknote);
            _unitOfWork.Save();

            Assert.Equal(6, _unitOfWork.BanknoteRepository.Get(resourceParameters).Count());
            Assert.Equal("Euros", _unitOfWork.BanknoteRepository.GetById(id).Type);
        }

        [Fact]
        public void Delete_RemovesBanknoteFromDatabase()
        {
            Guid id = new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c");
            Banknote banknote = _unitOfWork.BanknoteRepository.GetById(id);

            _unitOfWork.BanknoteRepository.Delete(banknote);
            _unitOfWork.Save();

            Assert.Equal(5, _unitOfWork.BanknoteRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.BanknoteRepository.GetById(id));
        }

        [Fact]
        public void Exists_ReturnsTrue_GivenValidBanknoteId()
        {
            var result = _unitOfWork.BanknoteRepository
                .Exists(new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89"));

            Assert.True(result);
        }

        [Fact]
        public void Exists_ReturnsFalse_GivenInvalidBanknoteId()
        {
            var result = _unitOfWork.BanknoteRepository
                .Exists(new Guid("358a071b-9bf7-49d8-ac50-3296684e3ea7"));

            Assert.False(result);
        }
    }
}