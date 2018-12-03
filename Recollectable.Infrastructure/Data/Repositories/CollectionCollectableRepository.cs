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
    public class CollectionCollectableRepository : IRepository<CollectionCollectable>
    {
        private RecollectableContext _context;

        public CollectionCollectableRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CollectionCollectable>> GetAll
            (Specification<CollectionCollectable> specification = null)
        {
            var collectables = _context.CollectionCollectables
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.Country)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.CollectorValue);

            return specification == null ?
                await collectables.ToListAsync() :
                await collectables.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<CollectionCollectable> GetSingle
            (Specification<CollectionCollectable> specification = null)
        {
            var collectables = _context.CollectionCollectables
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.Country)
                .Include(cc => cc.Collectable)
                .ThenInclude(c => c.CollectorValue);

            return specification == null ?
                await collectables.FirstOrDefaultAsync() :
                await collectables.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(CollectionCollectable collectable)
        {
            if (collectable.Id == Guid.Empty)
            {
                collectable.Id = Guid.NewGuid();
            }

            await _context.CollectionCollectables.AddAsync(collectable);
        }

        public void Delete(CollectionCollectable collectable)
        {
            _context.CollectionCollectables.Remove(collectable);
        }
    }
}