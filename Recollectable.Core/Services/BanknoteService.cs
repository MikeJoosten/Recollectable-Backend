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
        private readonly IUnitOfWork _unitOfWork;

        public BanknoteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Banknote>> FindBanknotes(CurrenciesResourceParameters resourceParameters)
        {
            var banknotes = await _unitOfWork.Banknotes.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                banknotes = await _unitOfWork.Banknotes.GetAll(new BanknoteByType(resourceParameters.Type));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                banknotes = await _unitOfWork.Banknotes.GetAll(new BanknoteByCountry(resourceParameters.Country));
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                banknotes = await _unitOfWork.Banknotes.GetAll(new BanknoteBySearch(resourceParameters.Search));
            }

            banknotes = banknotes.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CurrencyPropertyMapping);

            return PagedList<Banknote>.Create(banknotes.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<Banknote> FindBanknoteById(Guid id)
        {
            return await _unitOfWork.Banknotes.GetSingle(new BanknoteById(id));
        }

        public async Task CreateBanknote(Banknote banknote)
        {
            await _unitOfWork.Banknotes.Add(banknote);
        }

        public void UpdateBanknote(Banknote banknote) { }

        public void RemoveBanknote(Banknote banknote)
        {
            _unitOfWork.Banknotes.Delete(banknote);
        }

        public async Task<bool> BanknoteExists(Guid id)
        {
            var banknote = await _unitOfWork.Banknotes.GetSingle(new BanknoteById(id));
            return banknote != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}