using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Specifications.Locations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CountryRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllCountries()
        {
            //Act
            var result = await _unitOfWork.Countries.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCountry()
        {
            //Act
            var result = await _unitOfWork.Countries.GetSingle();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_AddsNewCountry()
        {
            //Arrange
            Guid id = new Guid("5de43b7d-3a80-4ad3-84ba-4f260bf94318");
            Country newCountry = new Country
            {
                Id = id,
                Name = "China"
            };

            //Act
            await _unitOfWork.Countries.Add(newCountry);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.Countries.GetAll()).Count());
            Assert.Equal("China", (await _unitOfWork.Countries.GetSingle(new CountryById(id))).Name);
        }

        [Fact]
        public async Task Delete_RemovesCountryFromDatabase()
        {
            //Arrange
            Guid id = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3");
            Country country = await _unitOfWork.Countries.GetSingle(new CountryById(id));

            //Act
            _unitOfWork.Countries.Delete(country);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.Countries.GetAll()).Count());
            Assert.Null(await _unitOfWork.Countries.GetSingle(new CountryById(id)));
        }
    }
}