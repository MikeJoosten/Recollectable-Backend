using Microsoft.EntityFrameworkCore;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class BanknoteRepository : IBanknoteRepository
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public BanknoteRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Banknote> GetBanknotes
            (CurrenciesResourceParameters resourceParameters)
        {
            var banknotes = _context.Banknotes
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<BanknoteDto, Banknote>());

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