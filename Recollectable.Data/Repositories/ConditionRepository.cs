using Microsoft.EntityFrameworkCore;
using Recollectable.Data.Helpers;
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

        public IEnumerable<Condition> GetConditions
            (ConditionsResourceParameters resourceParameters)
        {
            var conditions = _context.Conditions
                .OrderBy(c => c.Grade)
                .AsQueryable();

            if (!string.IsNullOrEmpty(resourceParameters.Grade))
            {
                var grade = resourceParameters.Grade.Trim().ToLowerInvariant();
                conditions = conditions.Where(c => c.Grade.ToLowerInvariant() == grade);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                conditions = conditions.Where(c => c.Grade.ToLowerInvariant().Contains(search));
            }

            return conditions;
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

        public bool ConditionExists(Guid conditionId)
        {
            return _context.Conditions.Any(c => c.Id == conditionId);
        }
    }
}