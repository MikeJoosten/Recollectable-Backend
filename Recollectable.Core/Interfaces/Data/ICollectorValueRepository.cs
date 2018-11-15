using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.ResourceParameters;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface ICollectorValueRepository : IRepository<CollectorValue, CollectorValuesResourceParameters>
    {
        Task<CollectorValue> FindDuplicate(CollectorValue collectorValue);
    }
}