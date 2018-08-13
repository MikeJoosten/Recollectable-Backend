using Recollectable.Data.Helpers;
using Recollectable.Data.Services;
using Recollectable.Domain.Entities;
using Recollectable.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Data.Repositories
{
    public class ConditionRepository : IConditionRepository
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public ConditionRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public IEnumerable<Condition> GetConditions
            (ConditionsResourceParameters resourceParameters)
        {
            var conditions = _context.Conditions.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<ConditionDto, Condition>());

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