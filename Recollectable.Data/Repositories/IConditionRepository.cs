using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IConditionRepository
    {
        IEnumerable<Condition> GetConditions();
        Condition GetCondition(Guid conditionId);
        void AddCondition(Condition condition);
        void UpdateCondition(Condition condition);
        void DeleteCondition(Condition condition);
    }
}