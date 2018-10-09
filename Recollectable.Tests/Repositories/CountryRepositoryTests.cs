using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CountryRepositoryTests : RecollectableTestBase
    {
        private CountriesResourceParameters resourceParameters;

        public CountryRepositoryTests()
        {
            resourceParameters = new CountriesResourceParameters();
        }

        [Fact]
        public void Get_ReturnsAllCountries()
        {
            //Act
            var result = _unitOfWork.CountryRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void Get_OrdersCountriesByName()
        {
            //Act
            var result = _unitOfWork.CountryRepository.Get(resourceParameters);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Canada", result.First().Name);
        }

        [Fact]
        public void GetById_ReturnsCountry_GivenValidId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var result = _unitOfWork.CountryRepository.GetById(id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public void GetById_ReturnsNull_GivenInvalidId()
        {
            //Arrange
            Guid id = new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc");

            //Act
            var result = _unitOfWork.CountryRepository.GetById(id);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsNewCountry()
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
            _unitOfWork.Save();

            //Assert
            Assert.Equal(7, _unitOfWork.CountryRepository.Get(resourceParameters).Count());
            Assert.Equal("China", _unitOfWork.CountryRepository.GetById(id).Name);
        }

        [Fact]
        public void Update_UpdatesExistingCountry()
        {
            //Arrange
            Guid id = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0");
            Country updatedCountry = _unitOfWork.CountryRepository.GetById(id);
            updatedCountry.Name = "China";

            //Act
            _unitOfWork.CountryRepository.Update(updatedCountry);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(6, _unitOfWork.CountryRepository.Get(resourceParameters).Count());
            Assert.Equal("China", _unitOfWork.CountryRepository.GetById(id).Name);
        }

        [Fact]
        public void Delete_RemovesCountryFromDatabase()
        {
            //Arrange
            Guid id = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3");
            Country country = _unitOfWork.CountryRepository.GetById(id);

            //Act
            _unitOfWork.CountryRepository.Delete(country);
            _unitOfWork.Save();

            //Assert
            Assert.Equal(5, _unitOfWork.CountryRepository.Get(resourceParameters).Count());
            Assert.Null(_unitOfWork.CountryRepository.GetById(id));
        }

        [Fact]
        public void Exists_ReturnsTrue_GivenValidCountryId()
        {
            //Arrange
            Guid id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4");

            //Act
            var result = _unitOfWork.CountryRepository.Exists(id);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_ReturnsFalse_GivenInvalidCountryId()
        {
            //Arrange
            Guid id = new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc");

            //Act
            var result = _unitOfWork.CountryRepository.Exists(id);

            //Assert
            Assert.False(result);
        }
    }
}