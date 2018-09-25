using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectorValueRepository : 
        BaseRepository<CollectorValue, CollectorValuesResourceParameters>
    {
        private RecollectableContext _context;

        public CollectorValueRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<CollectorValue> Get
            (CollectorValuesResourceParameters resourceParameters)
        {
            var collectorValues = _context.CollectorValues.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CollectorValueDto, CollectorValue>());

            return PagedList<CollectorValue>.Create(collectorValues,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public override CollectorValue GetById(Guid collectorValueId)
        {
            return _context.CollectorValues.FirstOrDefault(c => c.Id == collectorValueId);
        }

        public override void Add(CollectorValue collectorValue)
        {
            if (collectorValue.Id == Guid.Empty)
            {
                collectorValue.Id = Guid.NewGuid();
            }

            _context.CollectorValues.Add(collectorValue);
        }

        public override void Update(CollectorValue collectorValue) { }

        public override void Delete(CollectorValue collectorValue)
        {
            _context.CollectorValues.Remove(collectorValue);
        }

        public override bool Exists(Guid collectorValueId)
        {
            return _context.CollectorValues.Any(c => c.Id == collectorValueId);
        }
    }
}