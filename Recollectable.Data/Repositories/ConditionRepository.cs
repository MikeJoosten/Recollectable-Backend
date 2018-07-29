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

        public Condition GetCondition(Guid conditionId)
        {
            return _context.Conditions.FirstOrDefault(c => c.Id == conditionId);
        }

        public Condition GetConditionByCollectable(Guid collectionId, Guid collectableId)
        {
            return _context.Conditions
                .Include(c => c.CollectionCollectables)
                .ThenInclude(cc => cc.CollectionId == collectionId && 
                    cc.CollectableId == collectableId)
                .FirstOrDefault();
        }

        public void AddCondition(Condition condition)
        {
            condition.Id = Guid.NewGuid();
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