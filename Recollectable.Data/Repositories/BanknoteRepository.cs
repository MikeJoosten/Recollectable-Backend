using Microsoft.EntityFrameworkCore;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class BanknoteRepository : IBanknoteRepository
    {
        private RecollectableContext _context;
        private ICountryRepository _countryRepository;
        private ICollectionRepository _collectionRepository;
        private IConditionRepository _conditionRepository;

        public BanknoteRepository(RecollectableContext context,
            ICountryRepository countryRepository,
            ICollectionRepository collectionRepository,
            IConditionRepository conditionRepository)
        {
            _context = context;
            _countryRepository = countryRepository;
            _collectionRepository = collectionRepository;
            _conditionRepository = conditionRepository;
        }

        public IEnumerable<Banknote> GetBanknotes()
        {
            return _context.Banknotes.OrderBy(b => b.Country.Name);
        }

        public IEnumerable<Banknote> GetBanknotesByCountry(Guid countryId)
        {
            if (_countryRepository.GetCountry(countryId) == null)
            {
                return null;
            }

            return _context.Banknotes.Where(b => b.CountryId == countryId);
        }

        public IEnumerable<Banknote> GetBanknotesByCollection(Guid collectionId)
        {
            Collection collection = _collectionRepository.GetCollection(collectionId);

            if (collection == null || collection.Type != "Banknote")
            {
                return null;
            }

            return _context.CollectionCollectables
                .Include(cc => cc.Collectable)
                .Where(cc => cc.CollectionId == collectionId)
                .Select(cc => (Banknote)cc.Collectable)
                .OrderBy(c => c.Country.Name);
        }

        public Banknote GetBanknote(Guid banknoteId)
        {
            return _context.Banknotes.FirstOrDefault(b => b.Id == banknoteId);
        }

        public void AddBanknote(Banknote banknote)
        {
            if (banknote.Id == Guid.Empty)
            {
                banknote.Id = Guid.NewGuid();
            }

            _context.Banknotes.Add(banknote);
        }

        public void AddBanknoteToCollection(CollectionCollectable collectionCollectable)
        {
            Collection collection = _collectionRepository
                .GetCollection(collectionCollectable.CollectionId);
            Condition condition = _conditionRepository
                .GetCondition(collectionCollectable.ConditionId);
            Banknote banknote = GetBanknote(collectionCollectable.CollectableId);

            if (collection != null && banknote != null &&
                condition != null && collection.Type == "Banknote")
            {
                _context.Add(collectionCollectable);
            }
        }

        public void UpdateBanknote(Banknote banknote)
        {
            _context.Banknotes.Update(banknote);
        }

        public void DeleteBanknote(Banknote banknote)
        {
            _context.Banknotes.Remove(banknote);
        }

        public void DeleteBanknoteFromCollection(CollectionCollectable collectionCollectable)
        {
            _context.Remove(collectionCollectable);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}