using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IConditionService
    {
        Task<PagedList<Condition>> FindConditions(ConditionsResourceParameters resourceParameters);
        Task<Condition> FindConditionById(Guid id);
        Task CreateCondition(Condition condition);
        void UpdateCondition(Condition condition);
        void RemoveCondition(Condition condition);
        Task<bool> ConditionExists(Guid id);
        Task<bool> Save();
    }
}