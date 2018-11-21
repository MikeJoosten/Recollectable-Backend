using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface ICountryService
    {
        Task<PagedList<Country>> FindCountries(CountriesResourceParameters resourceParameters);
        Task<Country> FindCountryById(Guid id);
        Task CreateCountry(Country country);
        void UpdateCountry(Country country);
        void RemoveCountry(Country country);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}