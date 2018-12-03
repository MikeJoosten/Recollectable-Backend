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
    public class BanknoteRepository : IRepository<Banknote>
    {
        private RecollectableContext _context;

        public BanknoteRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Banknote>> GetAll(Specification<Banknote> specification = null)
        {
            var banknotes = _context.Banknotes
                .Include(c => c.Country)
                .Include(c => c.CollectorValue);

            return specification == null ?
                await banknotes.ToListAsync() :
                await banknotes.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<Banknote> GetSingle(Specification<Banknote> specification = null)
        {
            var banknotes = _context.Banknotes
                .Include(c => c.Country)
                .Include(c => c.CollectorValue);

            return specification == null ?
                await banknotes.FirstOrDefaultAsync() :
                await banknotes.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(Banknote banknote)
        {
            if (banknote.Id == Guid.Empty)
            {
                banknote.Id = Guid.NewGuid();
            }

            await _context.Banknotes.AddAsync(banknote);
        }

        public void Delete(Banknote banknote)
        {
            _context.Banknotes.Remove(banknote);
        }
    }
}