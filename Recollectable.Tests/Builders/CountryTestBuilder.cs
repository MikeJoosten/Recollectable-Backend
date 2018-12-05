using Recollectable.API.Models.Locations;
using Recollectable.Core.Entities.Locations;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class CountryTestBuilder
    {
        private Country country;

        public CountryTestBuilder()
        {
            country = new Country();
        }

        public CountryTestBuilder WithId(Guid id)
        {
            country.Id = id;
            return this;
        }

        public CountryTestBuilder WithName(string name)
        {
            country.Name = name;
            return this;
        }

        public Country Build()
        {
            return country;
        }

        public CountryCreationDto BuildCreationDto()
        {
            return new CountryCreationDto
            {
                Name = country.Name
            };
        }

        public CountryUpdateDto BuildUpdateDto()
        {
            return new CountryUpdateDto
            {
                Name = country.Name
            };
        }

        public List<Country> Build(int count)
        {
            var countries = new List<Country>();

            for (int i = 0; i < count; i++)
            {
                countries.Add(country);
            }

            return countries;
        }
    }
}