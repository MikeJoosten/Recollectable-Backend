using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Comparers;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectorValueRepository : ICollectorValueRepository
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CollectorValueRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<CollectorValue>> GetCollectorValues
            (CollectorValuesResourceParameters resourceParameters)
        {
            var collectorValues = await _context.CollectorValues.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CollectorValueDto, CollectorValue>())
                .ToListAsync();

            return PagedList<CollectorValue>.Create(collectorValues,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<CollectorValue> GetCollectorValueById(Guid collectorValueId)
        {
            return await _context.CollectorValues.FirstOrDefaultAsync(c => c.Id == collectorValueId);
        }

        public async Task<CollectorValue> GetCollectorValueByValues(CollectorValue collectorValue)
        {
            return await _context.CollectorValues
                .SingleOrDefaultAsync(c => new CollectorValueComparer().Equals(c, collectorValue));
        }

        public void AddCollectorValue(CollectorValue collectorValue)
        {
            if (collectorValue.Id == Guid.Empty)
            {
                collectorValue.Id = Guid.NewGuid();
            }

            _context.CollectorValues.Add(collectorValue);
        }

        public void UpdateCollectorValue(CollectorValue collectorValue) { }

        public void DeleteCollectorValue(CollectorValue collectorValue)
        {
            _context.CollectorValues.Remove(collectorValue);
        }

        public async Task<bool> Exists(Guid collectorValueId)
        {
            return await _context.CollectorValues.AnyAsync(c => c.Id == collectorValueId);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}