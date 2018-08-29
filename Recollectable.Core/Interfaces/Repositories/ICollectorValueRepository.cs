using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface ICollectorValueRepository
    {
        PagedList<CollectorValue> GetCollectorValues
            (CollectorValuesResourceParameters resourceParameters);
        CollectorValue GetCollectorValue(Guid collectorValueId);
        void AddCollectorValue(CollectorValue collectorValue);
        void UpdateCollectorValue(CollectorValue collectorValue);
        void DeleteCollectorValue(CollectorValue collectorValue);
        bool Save();
        bool CollectorValueExists(Guid collectorValueId);
    }
}