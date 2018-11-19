using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICollectorValueRepository
    {
        Task<PagedList<CollectorValue>> GetCollectorValues(CollectorValuesResourceParameters resourceParameters);
        Task<CollectorValue> GetCollectorValueById(Guid id);
        Task<CollectorValue> GetCollectorValueByValues(CollectorValue collectorValue);
        void AddCollectorValue(CollectorValue collectorValue);
        void UpdateCollectorValue(CollectorValue collectorValue);
        void DeleteCollectorValue(CollectorValue collectorValue);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}