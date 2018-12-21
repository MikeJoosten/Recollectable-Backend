using LinqSpecs.Core;
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class ConditionRepository : IRepository<Condition>
    {
        private RecollectableContext _context;

        public ConditionRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Condition>> GetAll(Specification<Condition> specification = null)
        {
            var conditions = _context.Conditions;

            return specification == null ?
                await conditions.ToListAsync() :
                await conditions.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<Condition> GetSingle(Specification<Condition> specification = null)
        {
            var conditions = _context.Conditions;

            return specification == null ?
                await conditions.FirstOrDefaultAsync() :
                await conditions.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(Condition condition)
        {
            if (condition.Id == Guid.Empty)
            {
                condition.Id = Guid.NewGuid();
            }

            await _context.Conditions.AddAsync(condition);
        }

        public void Delete(Condition condition)
        {
            _context.Conditions.Remove(condition);
        }
    }
}