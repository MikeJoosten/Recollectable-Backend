using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface IBanknoteRepository
    {
        Task<PagedList<Banknote>> GetBanknotes(CurrenciesResourceParameters resourceParameters);
        Task<Banknote> GetBanknoteById(Guid id);
        void AddBanknote(Banknote banknote);
        void UpdateBanknote(Banknote banknote);
        void DeleteBanknote(Banknote banknote);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}