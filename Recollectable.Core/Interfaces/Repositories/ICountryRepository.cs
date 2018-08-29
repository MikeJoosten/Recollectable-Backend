using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
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