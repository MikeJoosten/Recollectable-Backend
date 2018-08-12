using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IConditionRepository
    {
        IEnumerable<Condition> GetConditions();
        IEnumerable<Condition> GetConditionsByCollectable
            (Guid collectionId, Guid collectableId);
        Condition GetCondition(Guid conditionId);
        void AddCondition(Condition condition);
        void UpdateCondition(Condition condition);
        void DeleteCondition(Condition condition);
        bool Save();
        bool ConditionExists(Guid conditionId);
    }
}