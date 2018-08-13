using Recollectable.Data.Helpers;
using Recollectable.Data.Services;
using Recollectable.Domain.Entities;
using Recollectable.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Data.Repositories
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

        public PagedList<CollectorValue> GetCollectorValues
            (CollectorValuesResourceParameters resourceParameters)
        {
            var collectorValues = _context.CollectorValues.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CollectorValueDto, CollectorValue>());

            return PagedList<CollectorValue>.Create(collectorValues,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public CollectorValue GetCollectorValue(Guid collectorValueId)
        {
            return _context.CollectorValues.FirstOrDefault(c => c.Id == collectorValueId);
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

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool CollectorValueExists(Guid collectorValueId)
        {
            return _context.CollectorValues.Any(c => c.Id == collectorValueId);
        }
    }
}