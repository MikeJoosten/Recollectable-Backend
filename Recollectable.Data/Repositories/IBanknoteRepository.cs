using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IBanknoteRepository
    {
        IEnumerable<Banknote> GetBanknotes();
        IEnumerable<Banknote> GetBanknotesByCollection(Guid collectionId);
        Banknote GetBanknote(Guid banknoteId);
        Banknote GetBanknoteByCollection(Guid collectionId, Guid banknoteId);
        void AddBanknote(Banknote banknote);
        void UpdateBanknote(Banknote banknote);
        void DeleteBanknote(Banknote banknote);
    }
}