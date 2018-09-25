using Recollectable.Core.DTOs.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class ConditionRepository : BaseRepository<Condition, ConditionsResourceParameters>
    {
        private RecollectableContext _context;

        public ConditionRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<Condition> Get(ConditionsResourceParameters resourceParameters)
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

        public override Condition GetById(Guid conditionId)
        {
            return _context.Conditions.FirstOrDefault(c => c.Id == conditionId);
        }

        public override void Add(Condition condition)
        {
            if (condition.Id == Guid.Empty)
            {
                condition.Id = Guid.NewGuid();
            }

            _context.Conditions.Add(condition);
        }

        public override void Update(Condition condition) { }

        public override void Delete(Condition condition)
        {
            _context.Conditions.Remove(condition);
        }

        public override bool Exists(Guid conditionId)
        {
            return _context.Conditions.Any(c => c.Id == conditionId);
        }
    }
}