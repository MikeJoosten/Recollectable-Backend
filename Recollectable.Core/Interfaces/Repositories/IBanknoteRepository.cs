using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface IBanknoteRepository
    {
        PagedList<Banknote> GetBanknotes(CurrenciesResourceParameters resourceParameters);
        Banknote GetBanknote(Guid banknoteId);
        void AddBanknote(Banknote banknote);
        void UpdateBanknote(Banknote banknote);
        void DeleteBanknote(Banknote banknote);
        bool Save();
        bool BanknoteExists(Guid banknoteId);
    }
}