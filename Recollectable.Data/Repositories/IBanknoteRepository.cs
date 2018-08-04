using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IBanknoteRepository
    {
        IEnumerable<Banknote> GetBanknotes();
        IEnumerable<Banknote> GetBanknotesByCountry(Guid countryId);
        IEnumerable<Banknote> GetBanknotesByCollection(Guid collectionId);
        Banknote GetBanknote(Guid banknoteId);
        void AddBanknote(Banknote banknote);
        void AddBanknoteToCollection(CollectionCollectable collectionCollectable);
        void UpdateBanknote(Banknote banknote);
        void DeleteBanknote(Banknote banknote);
        void DeleteBanknoteFromCollection(CollectionCollectable collectionCollectable);
        bool Save();
    }
}