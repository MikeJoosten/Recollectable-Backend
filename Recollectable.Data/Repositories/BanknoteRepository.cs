using Microsoft.EntityFrameworkCore;
using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public PagedList<Banknote> GetBanknotes
            (CollectablesResourceParameters resourceParameters)
        {
            var banknotes = _context.Banknotes
                .Include(b => b.Country)
                .Include(b => b.CollectorValue)
                .OrderBy(b => b.Country.Name)
                .ThenBy(b => (b.FaceValue + " " + b.Type))
                .ThenBy(b => b.ReleaseDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Type.ToLowerInvariant() == type);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Country.Name.ToLowerInvariant() == country);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Country.Name.ToLowerInvariant().Contains(search)
                    || b.Type.ToLowerInvariant().Contains(search)
                    || b.ReleaseDate.ToLowerInvariant().Contains(search)
                    || b.Color.ToLowerInvariant().Contains(search));
            }

            return PagedList<Banknote>.Create(banknotes,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public IEnumerable<Banknote> GetBanknotesByCountry(Guid countryId)
        {
            if (_countryRepository.GetCountry(countryId) == null)
            {
                return null;
            }

            return _context.Banknotes
                .Include(b => b.Country)
                .Include(b => b.CollectorValue)
                .Where(b => b.CountryId == countryId)
                .OrderBy(b => (b.FaceValue + " " + b.Type))
                .ThenBy(b => b.ReleaseDate);
        }

        public Banknote GetBanknote(Guid banknoteId)
        {
            return _context.Banknotes
                .Include(b => b.Country)
                .Include(b => b.CollectorValue)
                .FirstOrDefault(b => b.Id == banknoteId);
        }

        public void AddBanknote(Banknote banknote)
        {
            if (banknote.Id == Guid.Empty)
            {
                banknote.Id = Guid.NewGuid();
            }

            if (banknote.CountryId == Guid.Empty)
            {
                banknote.CountryId = Guid.NewGuid();
            }

            if (banknote.CollectorValueId == Guid.Empty)
            {
                banknote.CollectorValueId = Guid.NewGuid();
            }

            _context.Banknotes.Add(banknote);
        }

        public void UpdateBanknote(Banknote banknote) { }

        public void DeleteBanknote(Banknote banknote)
        {
            _context.Banknotes.Remove(banknote);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool BanknoteExists(Guid banknoteId)
        {
            return _context.Banknotes.Any(b => b.Id == banknoteId);
        }
    }
}