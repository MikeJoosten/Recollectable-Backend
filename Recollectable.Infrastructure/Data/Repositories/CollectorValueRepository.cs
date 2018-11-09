using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collectables;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
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

        public async Task<CollectorValue> GetByValues(CollectorValue collectorValue)
        {
            return await _context.CollectorValues.Where(c =>
                c.G4 == collectorValue.G4 && c.VG8 == collectorValue.VG8 && 
                c.F12 == collectorValue.F12 && c.VF20 == collectorValue.VF20 && 
                c.XF40 == collectorValue.XF40 && c.AU50 == collectorValue.AU50 &&
                c.MS60 == collectorValue.MS60 && c.MS63 == collectorValue.MS63 && 
                c.PF60 == collectorValue.PF60 && c.PF63 == collectorValue.PF63 && 
                c.PF65 == collectorValue.PF65).FirstOrDefaultAsync();
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