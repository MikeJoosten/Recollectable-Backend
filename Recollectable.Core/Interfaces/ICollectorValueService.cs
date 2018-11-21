using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface ICollectorValueService
    {
        Task<PagedList<CollectorValue>> FindCollectorValues(CollectorValuesResourceParameters resourceParameters);
        Task<CollectorValue> FindCollectorValueById(Guid id);
        Task<CollectorValue> FindCollectorValueByValues(CollectorValue collectorValue);
        Task CreateCollectorValue(CollectorValue collectorValue);
        void UpdateCollectorValue(CollectorValue collectorValue);
        void RemoveCollectorValue(CollectorValue collectorValue);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}