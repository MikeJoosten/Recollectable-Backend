using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using System;

namespace Recollectable.Core.Interfaces.Repositories
{
    public interface IConditionRepository
    {
        PagedList<Condition> GetConditions
            (ConditionsResourceParameters resourceParameters);
        Condition GetCondition(Guid conditionId);
        void AddCondition(Condition condition);
        void UpdateCondition(Condition condition);
        void DeleteCondition(Condition condition);
        bool Save();
        bool ConditionExists(Guid conditionId);
    }
}