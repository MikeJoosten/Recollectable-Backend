using Recollectable.Data.Helpers;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IBanknoteRepository
    {
        PagedList<Banknote> GetBanknotes(CollectablesResourceParameters resourceParameters);
        IEnumerable<Banknote> GetBanknotesByCountry(Guid countryId);
        Banknote GetBanknote(Guid banknoteId);
        void AddBanknote(Banknote banknote);
        void UpdateBanknote(Banknote banknote);
        void DeleteBanknote(Banknote banknote);
        bool Save();
        bool BanknoteExists(Guid banknoteId);
    }
}