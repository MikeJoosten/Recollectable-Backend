using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICountryRepository
    {
        Task<PagedList<Country>> GetCountries(CountriesResourceParameters resourceParameters);
        Task<Country> GetCountryById(Guid id);
        void AddCountry(Country country);
        void UpdateCountry(Country country);
        void DeleteCountry(Country country);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}