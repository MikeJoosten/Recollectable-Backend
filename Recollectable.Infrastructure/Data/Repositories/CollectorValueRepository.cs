using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectorValueRepository : 
        IRepository<CollectorValue, CollectorValuesResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CollectorValueRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<CollectorValue>> Get
            (CollectorValuesResourceParameters resourceParameters)
        {
            var collectorValues = await _context.CollectorValues.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CollectorValueDto, CollectorValue>())
                .ToListAsync();

            return PagedList<CollectorValue>.Create(collectorValues,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<CollectorValue> GetById(Guid collectorValueId)
        {
            return await _context.CollectorValues.FirstOrDefaultAsync(c => c.Id == collectorValueId);
        }

        public void Add(CollectorValue collectorValue)
        {
            if (collectorValue.Id == Guid.Empty)
            {
                collectorValue.Id = Guid.NewGuid();
            }

            _context.CollectorValues.Add(collectorValue);
        }

        public void Update(CollectorValue collectorValue) { }

        public void Delete(CollectorValue collectorValue)
        {
            _context.CollectorValues.Remove(collectorValue);
        }

        public async Task<bool> Exists(Guid collectorValueId)
        {
            return await _context.CollectorValues.AnyAsync(c => c.Id == collectorValueId);
        }
    }
}