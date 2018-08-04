using Microsoft.EntityFrameworkCore;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class ConditionRepository : IConditionRepository
    {
        private RecollectableContext _context;

        public ConditionRepository(RecollectableContext context)
        {
            _context = context;
        }

        public IEnumerable<Condition> GetConditions()
        {
            return _context.Conditions.OrderBy(c => c.Grade);
        }

        public IEnumerable<Condition> GetConditionsByCollectable
            (Guid collectionId, Guid collectableId)
        {
            return _context.CollectionCollectables
                .Include(cc => cc.Condition)
                .Where(cc => cc.CollectionId == collectionId && 
                    cc.CollectableId == collectableId)
                .Select(cc => cc.Condition)
                .OrderBy(c => c.Grade);
        }

        public Condition GetCondition(Guid conditionId)
        {
            return _context.Conditions.FirstOrDefault(c => c.Id == conditionId);
        }

        public void AddCondition(Condition condition)
        {
            if (condition.Id == Guid.Empty)
            {
                condition.Id = Guid.NewGuid();
            }

            _context.Conditions.Add(condition);
        }

        public void UpdateCondition(Condition condition) { }

        public void DeleteCondition(Condition condition)
        {
            _context.Conditions.Remove(condition);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}