using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CountryRepositoryTests : RecollectableTestBase
    {
        private CountriesResourceParameters resourceParameters;

        /*public CountryRepositoryTests()
        {
            resourceParameters = new CountriesResourceParameters();
        }

        [Fact]
        public async Task Get_ReturnsAllCountries()
        {
            //Act
            var result = await _unitOfWork.CountryRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task Get_OrdersCountriesByName()
        {
            //Act
            var result = await _unitOfWork.CountryRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Canada", result.First().Name);
        }

        [Fact]
        public async Task GetById_ReturnsCountry_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var result = await _unitOfWork.CountryRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc");

            //Act
            var result = await _unitOfWork.CountryRepository.GetById(id);

            //Assert
            Assert.Null(result);
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
            _unitOfWork.CountryRepository.Add(newCountry);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.CountryRepository.Get(resourceParameters)).Count());
            Assert.Equal("China", (await _unitOfWork.CountryRepository.GetById(id)).Name);
        }

        [Fact]
        public async Task Update_UpdatesExistingCountry()
        {
            //Arrange
            Guid id = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0");
            Country updatedCountry = await _unitOfWork.CountryRepository.GetById(id);
            updatedCountry.Name = "China";

            //Act
            _unitOfWork.CountryRepository.Update(updatedCountry);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _unitOfWork.CountryRepository.Get(resourceParameters)).Count());
            Assert.Equal("China", (await _unitOfWork.CountryRepository.GetById(id)).Name);
        }

        [Fact]
        public async Task Delete_RemovesCountryFromDatabase()
        {
            //Arrange
            Guid id = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3");
            Country country = await _unitOfWork.CountryRepository.GetById(id);

            //Act
            _unitOfWork.CountryRepository.Delete(country);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.CountryRepository.Get(resourceParameters)).Count());
            Assert.Null(await _unitOfWork.CountryRepository.GetById(id));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_GivenValidCountryId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var result = await _unitOfWork.CountryRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc");

            //Act
            var result = await _unitOfWork.CountryRepository.Exists(id);

            //Assert
            Assert.False(result);
        }*/
    }
}