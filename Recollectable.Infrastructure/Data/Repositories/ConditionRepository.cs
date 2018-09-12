using Recollectable.Core.DTOs.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class ConditionRepository : IRepository<Condition, ConditionsResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public ConditionRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Condition> Get(ConditionsResourceParameters resourceParameters)
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

            return PagedList<Condition>.Create(conditions,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public Condition GetById(Guid conditionId)
        {
            return _context.Conditions.FirstOrDefault(c => c.Id == conditionId);
        }

        public void Add(Condition condition)
        {
            if (condition.Id == Guid.Empty)
            {
                condition.Id = Guid.NewGuid();
            }

            _context.Conditions.Add(condition);
        }

        public void Update(Condition condition) { }

        public void Delete(Condition condition)
        {
            _context.Conditions.Remove(condition);
        }

        public bool Exists(Guid conditionId)
        {
            return _context.Conditions.Any(c => c.Id == conditionId);
        }
    }
}