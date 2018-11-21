using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Collectables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class CollectorValueService : ICollectorValueService
    {
        private readonly IRepository<CollectorValue> _collectorValueRepository;

        public CollectorValueService(IRepository<CollectorValue> collectorValueRepository)
        {
            _collectorValueRepository = collectorValueRepository;
        }

        public async Task<PagedList<CollectorValue>> FindCollectorValues(CollectorValuesResourceParameters resourceParameters)
        {
            var collectorValues = await _collectorValueRepository.GetAll();

            collectorValues = collectorValues.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectorValuePropertyMapping);

            return PagedList<CollectorValue>.Create(collectorValues.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<CollectorValue> FindCollectorValueById(Guid id)
        {
            return await _collectorValueRepository.GetSingle(new CollectorValueById(id));
        }

        public async Task<CollectorValue> FindCollectorValueByValues(CollectorValue collectorValue)
        {
            return await _collectorValueRepository.GetSingle(new CollectorValueByValues(collectorValue));
        }

        public async Task CreateCollectorValue(CollectorValue collectorValue)
        {
            await _collectorValueRepository.Add(collectorValue);
        }

        public void UpdateCollectorValue(CollectorValue collectorValue)
        {
            _collectorValueRepository.Update(collectorValue);
        }

        public void RemoveCollectorValue(CollectorValue collectorValue)
        {
            _collectorValueRepository.Delete(collectorValue);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _collectorValueRepository.Exists(new CollectorValueById(id));
        }

        public async Task<bool> Save()
        {
            return await _collectorValueRepository.Save();
        }
    }
}