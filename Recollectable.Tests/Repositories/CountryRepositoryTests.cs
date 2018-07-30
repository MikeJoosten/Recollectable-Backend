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
    }
}