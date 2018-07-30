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
    public class CountryRepositoryTests
    {
        private RecollectableContext _context;
        private ICountryRepository _repository;

        public CountryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RecollectableContext(options);
            _repository = new CountryRepository(_context);
            Seed();
        }

        [Fact]
        public void GetCountries_ReturnsAllCountries()
        {
            var result = _repository.GetCountries();
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        private void Seed()
        {
            var countries = new[]
            {
                new Country
                {
                    Id = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                    Name = "United States of America"
                },
                new Country
                {
                    Id = new Guid("1e6a79fa-f216-41a4-8efe-0b87e58d2b33"),
                    Name = "Ecuador"
                },
                new Country
                {
                    Id = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0"),
                    Name = "Canada"
                },
                new Country
                {
                    Id = new Guid("8c29c8a2-93ae-483d-8235-b0c728d3a034"),
                    Name = "Mexico"
                },
                new Country
                {
                    Id = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                    Name = "Cuba"
                },
                new Country
                {
                    Id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4"),
                    Name = "Brazil"
                }
            };

            _context.Countries.AddRange(countries);
            _context.SaveChanges();
        }
    }
}