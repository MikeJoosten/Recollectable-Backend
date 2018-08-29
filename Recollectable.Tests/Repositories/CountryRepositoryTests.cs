using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Infrastructure.Data.Repositories;
using System;
using System.Linq;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CountryRepositoryTests : RecollectableTestBase
    {
        private ICountryRepository _repository;
        private CountriesResourceParameters resourceParameters;

        public CountryRepositoryTests()
        {
            _repository = new CountryRepository(_context, _propertyMappingService);
            resourceParameters = new CountriesResourceParameters();
        }

        [Fact]
        public void GetCountries_ReturnsAllCountries()
        {
            var result = _repository.GetCountries(resourceParameters);
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCountries_OrdersCountriesByName()
        {
            var result = _repository.GetCountries(resourceParameters);
            Assert.Equal("Canada", result.First().Name);
        }

        [Fact]
        public void GetCountry_ReturnsCountry_GivenValidId()
        {
            var result = _repository
                .GetCountry(new Guid("8cef5964-01a4-40c7-9f16-28af109094d4"));
            Assert.NotNull(result);
            Assert.Equal("8cef5964-01a4-40c7-9f16-28af109094d4", result.Id.ToString());
            Assert.Equal("Japan", result.Name);
        }

        [Fact]
        public void GetCountry_ReturnsNull_GivenInvalidId()
        {
            var result = _repository
                .GetCountry(new Guid("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc"));
            Assert.Null(result);
        }

        [Fact]
        public void AddCountry_AddsNewCountry()
        {
            Country newCountry = new Country
            {
                Id = new Guid("5de43b7d-3a80-4ad3-84ba-4f260bf94318"),
                Name = "China"
            };

            _repository.AddCountry(newCountry);
            _repository.Save();

            Assert.Equal(7, _repository.GetCountries(resourceParameters).Count());
            Assert.Equal("China", _repository
                .GetCountry(new Guid("5de43b7d-3a80-4ad3-84ba-4f260bf94318"))
                .Name);
        }

        [Fact]
        public void UpdateCountry_UpdatesExistingCountry()
        {
            Country updatedCountry = _repository
                .GetCountry(new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0"));
            updatedCountry.Name = "China";

            _repository.UpdateCountry(updatedCountry);
            _repository.Save();

            Assert.Equal(6, _repository.GetCountries(resourceParameters).Count());
            Assert.Equal("China", _repository
                .GetCountry(new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0"))
                .Name);
        }

        [Fact]
        public void DeleteCountry_RemovesCountryFromDatabase()
        {
            Country country = _repository.GetCountry(new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"));

            _repository.DeleteCountry(country);
            _repository.Save();

            Assert.Equal(5, _repository.GetCountries(resourceParameters).Count());
            Assert.Null(_repository.GetCountry(new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")));
        }
    }
}