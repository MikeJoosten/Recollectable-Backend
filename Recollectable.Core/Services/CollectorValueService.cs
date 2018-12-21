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
        private readonly IUnitOfWork _unitOfWork;

        public CollectorValueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<CollectorValue>> FindCollectorValues(CollectorValuesResourceParameters resourceParameters)
        {
            var collectorValues = await _unitOfWork.CollectorValues.GetAll();

            collectorValues = collectorValues.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.CollectorValuePropertyMapping);

            return PagedList<CollectorValue>.Create(collectorValues.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<CollectorValue> FindCollectorValueById(Guid id)
        {
            return await _unitOfWork.CollectorValues.GetSingle(new CollectorValueById(id));
        }

        public async Task<CollectorValue> FindCollectorValueByValues(CollectorValue collectorValue)
        {
            return await _unitOfWork.CollectorValues.GetSingle(new CollectorValueByValues(collectorValue));
        }

        public async Task CreateCollectorValue(CollectorValue collectorValue)
        {
            await _unitOfWork.CollectorValues.Add(collectorValue);
        }

        public void UpdateCollectorValue(CollectorValue collectorValue) { }

        public void RemoveCollectorValue(CollectorValue collectorValue)
        {
            _unitOfWork.CollectorValues.Delete(collectorValue);
        }

        public async Task<bool> CollectorValueExists(Guid id)
        {
            var collectorValue = await _unitOfWork.CollectorValues.GetSingle(new CollectorValueById(id));
            return collectorValue != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}