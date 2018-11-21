using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class BanknoteService : IBanknoteService
    {
        private readonly IRepository<Banknote> _banknoteRepository;

        public BanknoteService(IRepository<Banknote> banknoteRepository)
        {
            _banknoteRepository = banknoteRepository;
        }

        public async Task<PagedList<Banknote>> FindBanknotes(CurrenciesResourceParameters resourceParameters)
        {
            var banknotes = await _banknoteRepository.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                banknotes = await _banknoteRepository.GetAll(new BanknoteByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                banknotes = await _banknoteRepository.GetAll(new BanknoteByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                banknotes = await _banknoteRepository.GetAll(new BanknoteBySearch(resourceParameters.Search));
            }

            banknotes = banknotes.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CurrencyPropertyMapping);

            return PagedList<Banknote>.Create(banknotes.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Banknote> FindBanknoteById(Guid id)
        {
            return await _banknoteRepository.GetSingle(new BanknoteById(id));
        }

        public async Task CreateBanknote(Banknote banknote)
        {
            await _banknoteRepository.Add(banknote);
        }

        public void UpdateBanknote(Banknote banknote)
        {
            _banknoteRepository.Update(banknote);
        }

        public void RemoveBanknote(Banknote banknote)
        {
            _banknoteRepository.Delete(banknote);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _banknoteRepository.Exists(new BanknoteById(id));
        }

        public async Task<bool> Save()
        {
            return await _banknoteRepository.Save();
        }
    }
}