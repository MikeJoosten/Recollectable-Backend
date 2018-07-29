using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}