using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class BanknoteRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllBanknotes()
        {
            //Act
            var result = await _unitOfWork.Banknotes.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsBanknote()
        {
            //Act
            var result = await _unitOfWork.Banknotes.GetSingle();

            //Assert
            Assert.NotNull(result);
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
            await _unitOfWork.Banknotes.Add(newBanknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.Banknotes.GetAll()).Count());
            Assert.Equal("Euros", (await _unitOfWork.Banknotes.GetSingle(new BanknoteById(id))).Type);
        }

        [Fact]
        public async Task Delete_RemovesBanknoteFromDatabase()
        {
            //Arrange
            Guid id = new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c");
            Banknote banknote = await _unitOfWork.Banknotes.GetSingle(new BanknoteById(id));

            //Act
            _unitOfWork.Banknotes.Delete(banknote);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.Banknotes.GetAll()).Count());
            Assert.Null(await _unitOfWork.Banknotes.GetSingle(new BanknoteById(id)));
        }
    }
}