using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Recollectable.Data.Repositories
{
    public interface ICountryRepository
    {
        PagedList<Country> GetCountries
            (CountriesResourceParameters resourceParameters);
        Country GetCountry(Guid countryId);
        void AddCountry(Country country);
        void UpdateCountry(Country country);
        void DeleteCountry(Country country);
        bool Save();
        bool CountryExists(Guid countryId);
    }
}