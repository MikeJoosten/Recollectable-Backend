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
        Condition GetConditionByCollectable(Guid collectionId, Guid collectableId);
        void AddCondition(Condition condition);
        void UpdateCondition(Condition condition);
        void DeleteCondition(Condition condition);
        bool Save();
    }
}