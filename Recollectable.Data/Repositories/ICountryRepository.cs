using Recollectable.Data.Helpers;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface ICountryRepository
    {
        IEnumerable<Country> GetCountries
            (CountriesResourceParameters resourceParameters);
        Country GetCountry(Guid countryId);
        void AddCountry(Country country);
        void UpdateCountry(Country country);
        void DeleteCountry(Country country);
        bool Save();
        bool CountryExists(Guid countryId);
    }
}