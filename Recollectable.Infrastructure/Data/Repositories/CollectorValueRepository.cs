using LinqSpecs.Core;
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectorValueRepository : IRepository<CollectorValue>
    {
        private RecollectableContext _context;

        public CollectorValueRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CollectorValue>> GetAll(Specification<CollectorValue> specification = null)
        {
            return specification == null ?
                await _context.CollectorValues.ToListAsync() :
                await _context.CollectorValues.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<CollectorValue> GetSingle(Specification<CollectorValue> specification = null)
        {
            return specification == null ?
                await _context.CollectorValues.FirstOrDefaultAsync() :
                await _context.CollectorValues.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(CollectorValue collectorValue)
        {
            if (collectorValue.Id == Guid.Empty)
            {
                collectorValue.Id = Guid.NewGuid();
            }

            await _context.CollectorValues.AddAsync(collectorValue);
        }

        public void Update(CollectorValue collectorValue) { }

        public void Delete(CollectorValue collectorValue)
        {
            _context.CollectorValues.Remove(collectorValue);
        }

        public async Task<bool> Exists(Specification<CollectorValue> specification = null)
        {
            return await _context.CollectorValues.AnyAsync(specification.ToExpression());
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}