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
    public class CollectionRepository : IRepository<Collection>
    {
        private RecollectableContext _context;

        public CollectionRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Collection>> GetAll(Specification<Collection> specification = null)
        {
            return specification == null ?
                await _context.Collections.ToListAsync() :
                await _context.Collections.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<Collection> GetSingle(Specification<Collection> specification = null)
        {
            return specification == null ?
                await _context.Collections.FirstOrDefaultAsync() :
                await _context.Collections.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(Collection collection)
        {
            if (collection.Id == Guid.Empty)
            {
                collection.Id = Guid.NewGuid();
            }

            await _context.Collections.AddAsync(collection);
        }

        public void Delete(Collection collection)
        {
            _context.Collections.Remove(collection);
        }
    }
}