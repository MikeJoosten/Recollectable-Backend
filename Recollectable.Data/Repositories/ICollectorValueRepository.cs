using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Recollectable.Data.Repositories
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