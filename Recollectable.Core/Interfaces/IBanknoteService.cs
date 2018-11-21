using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IBanknoteService
    {
        Task<PagedList<Banknote>> FindBanknotes(CurrenciesResourceParameters resourceParameters);
        Task<Banknote> FindBanknoteById(Guid id);
        Task CreateBanknote(Banknote banknote);
        void UpdateBanknote(Banknote banknote);
        void RemoveBanknote(Banknote banknote);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}