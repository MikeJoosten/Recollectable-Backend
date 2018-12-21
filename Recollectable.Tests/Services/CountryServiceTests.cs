using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Services
{
    public class CountryServiceTests : RecollectableTestBase
    {
        private readonly ICountryService _countryService;
        private CountriesResourceParameters resourceParameters;

        public CountryServiceTests()
        {
            _countryService = new CountryService(_unitOfWork);
            resourceParameters = new CountriesResourceParameters();
        }

        [Fact]
        public async Task FindCountries_ReturnsAllCountries()
        {
            //Act
            var result = await _countryService.FindCountries(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task FindCountries_OrdersCountriesByName()
        {
            //Act
            var result = await _countryService.FindCountries(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Canada", result.First().Name);
        }

        [Fact]
        public async Task FindCountryById_ReturnsCountry_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var result = await _countryService.FindCountryById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public async Task FindCountryById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc");

            //Act
            var result = await _countryService.FindCountryById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCountry_CreatesNewCountry()
        {
            //Arrange
            Guid id = new Guid("5de43b7d-3a80-4ad3-84ba-4f260bf94318");
            Country newCountry = new Country
            {
                Id = id,
                Name = "China"
            };

            //Act
            await _countryService.CreateCountry(newCountry);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _countryService.FindCountries(resourceParameters)).Count());
            Assert.Equal("China", (await _countryService.FindCountryById(id)).Name);
        }

        [Fact]
        public async Task UpdateCountry_UpdatesExistingCountry()
        {
            //Arrange
            Guid id = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0");
            Country updatedCountry = await _countryService.FindCountryById(id);
            updatedCountry.Name = "China";

            //Act
            _countryService.UpdateCountry(updatedCountry);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(6, (await _countryService.FindCountries(resourceParameters)).Count());
            Assert.Equal("China", (await _countryService.FindCountryById(id)).Name);
        }

        [Fact]
        public async Task RemoveCountry_RemovesCountryFromDatabase()
        {
            //Arrange
            Guid id = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3");
            Country country = await _countryService.FindCountryById(id);

            //Act
            _countryService.RemoveCountry(country);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _countryService.FindCountries(resourceParameters)).Count());
            Assert.Null(await _countryService.FindCountryById(id));
        }

        [Fact]
        public async Task CountryExists_ReturnsTrue_GivenValidCountryId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var result = await _countryService.CountryExists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CountryExists_ReturnsFalse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc");

            //Act
            var result = await _countryService.CountryExists(id);

            //Assert
            Assert.False(result);
        }
    }
}