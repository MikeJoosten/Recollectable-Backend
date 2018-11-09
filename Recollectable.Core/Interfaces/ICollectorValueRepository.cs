using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface ICollectorValueRepository : IRepository<CollectorValue, CollectorValuesResourceParameters>
    {
        Task<CollectorValue> GetByValues(CollectorValue collectorValue);
    }
}