using Microsoft.EntityFrameworkCore;
using Recollectable.Data;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CountryRepositoryTests : RecollectableTestBase
    {
        private ICountryRepository _repository;

        public CountryRepositoryTests()
        {
            _repository = new CountryRepository(_context);
        }

        [Fact]
        public void GetCountries_ReturnsAllCountries()
        {
            var result = _repository.GetCountries();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void GetCountries_OrdersCountriesByName()
        {
            var result = _repository.GetCountries();
            Assert.Equal("Canada", result.First().Name);
        }

        [Theory]
        [InlineData("74619fd9-898c-4250-b5c9-833ce2d599c0", "Canada")]
        [InlineData("8c29c8a2-93ae-483d-8235-b0c728d3a034", "Mexico")]
        [InlineData("8cef5964-01a4-40c7-9f16-28af109094d4", "Japan")]
        public void GetCountry_ReturnsCountry_GivenValidId(string countryId, string expected)
        {
            var result = _repository.GetCountry(new Guid(countryId));
            Assert.NotNull(result);
            Assert.Equal(countryId, result.Id.ToString());
            Assert.Equal(expected, result.Name);
        }

        [Theory]
        [InlineData("c798a076-6080-4d40-9b3a-76bf75dc02e9")]
        [InlineData("433c33f0-fa1c-443e-9259-0f24057a7127")]
        [InlineData("8eb32be5-1d34-48d6-92ca-9049ef6ab0bc")]
        public void GetCountry_ReturnsNull_GivenInvalidId(string countryId)
        {
            var result = _repository.GetCountry(new Guid(countryId));
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

            Assert.Equal(7, _repository.GetCountries().Count());
            Assert.Equal("China", _repository
                .GetCountry(new Guid("5de43b7d-3a80-4ad3-84ba-4f260bf94318"))
                .Name);
        }

        [Theory]
        [InlineData("c8f2031e-c780-4d27-bf13-1ee48a7207a3", "United Kingdom")]
        [InlineData("74619fd9-898c-4250-b5c9-833ce2d599c0", "China")]
        [InlineData("8cef5964-01a4-40c7-9f16-28af109094d4", "Japan")]
        public void UpdateCountry_UpdatesExistingCountry(string countryId, string updatedName)
        {
            Country updatedCountry = _repository.GetCountry(new Guid(countryId));
            updatedCountry.Name = updatedName;

            _repository.UpdateCountry(updatedCountry);
            _repository.Save();

            Assert.Equal(6, _repository.GetCountries().Count());
            Assert.Equal(updatedName, _repository
                .GetCountry(new Guid(countryId))
                .Name);
        }

        [Fact]
        public void DeleteCountry_RemovesCountryFromDatabase()
        {
            Country country = _repository.GetCountry(new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"));

            _repository.DeleteCountry(country);
            _repository.Save();

            Assert.Equal(5, _repository.GetCountries().Count());
            Assert.Null(_repository.GetCountry(new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")));
        }
    }
}