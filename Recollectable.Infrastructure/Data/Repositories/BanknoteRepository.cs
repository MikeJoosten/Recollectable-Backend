using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class BanknoteRepository : IRepository<Banknote, CurrenciesResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public BanknoteRepository(RecollectableContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<Banknote>> Get(CurrenciesResourceParameters resourceParameters)
        {
            var banknotes = await _context.Banknotes
                .Include(c => c.Country)
                .Include(c => c.CollectorValue)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<BanknoteDto, Banknote>())
                .ToListAsync();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Type.ToLowerInvariant() == type).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Country))
            {
                var country = resourceParameters.Country.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Country.Name.ToLowerInvariant() == country).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                banknotes = banknotes.Where(b => b.Country.Name.ToLowerInvariant().Contains(search)
                    || b.Type.ToLowerInvariant().Contains(search)
                    || b.ReleaseDate.ToLowerInvariant().Contains(search)
                    || b.Color.ToLowerInvariant().Contains(search)).ToList();
            }

            return PagedList<Banknote>.Create(banknotes,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<Banknote> GetById(Guid banknoteId)
        {
            return await _context.Banknotes
                .Include(b => b.Country)
                .Include(b => b.CollectorValue)
                .FirstOrDefaultAsync(b => b.Id == banknoteId);
        }

        public void Add(Banknote banknote)
        {
            if (banknote.Id == Guid.Empty)
            {
                banknote.Id = Guid.NewGuid();
            }

            _context.Banknotes.Add(banknote);
        }

        public void Update(Banknote banknote) { }

        public void Delete(Banknote banknote)
        {
            _context.Banknotes.Remove(banknote);
        }

        public async Task<bool> Exists(Guid banknoteId)
        {
            return await _context.Banknotes.AnyAsync(b => b.Id == banknoteId);
        }
    }
}