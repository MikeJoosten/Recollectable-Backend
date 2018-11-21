using LinqSpecs.Core;
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectableRepository : IRepository<Collectable>
    {
        private RecollectableContext _context;

        public CollectableRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Collectable>> GetAll(Specification<Collectable> specification = null)
        {
            var collectableItems = _context.Collectables
                .Include(c => c.Country)
                .Include(c => c.CollectorValue);

            return specification == null ?
                await collectableItems.ToListAsync() :
                await collectableItems.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<Collectable> GetSingle(Specification<Collectable> specification = null)
        {
            var collectables = _context.Collectables
                .Include(c => c.Country)
                .Include(c => c.CollectorValue);

            return specification == null ?
                await collectables.FirstOrDefaultAsync() :
                await collectables.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(Collectable collectable)
        {
            if (collectable.Id == Guid.Empty)
            {
                collectable.Id = Guid.NewGuid();
            }

            await _context.Collectables.AddAsync(collectable);
        }

        public void Update(Collectable collectable) { }

        public void Delete(Collectable collectable)
        {
            _context.Collectables.Remove(collectable);
        }

        public async Task<bool> Exists(Specification<Collectable> specification = null)
        {
            return await _context.Collectables.AnyAsync(specification.ToExpression());
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}