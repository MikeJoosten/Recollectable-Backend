using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Recollectable.Data.Repositories
{
    public interface ICollectorValueRepository
    {
        IEnumerable<CollectorValue> GetCollectorValues();
        CollectorValue GetCollectorValue(Guid collectorValueId);
        void AddCollectorValue(CollectorValue collectorValue);
        void UpdateCollectorValue(CollectorValue collectorValue);
        void DeleteCollectorValue(CollectorValue collectorValue);
        bool Save();
        bool CollectorValueExists(Guid collectorValueId);
    }
}