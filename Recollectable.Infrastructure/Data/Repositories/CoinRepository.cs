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
    public class CoinRepository : IRepository<Coin>
    {
        private RecollectableContext _context;

        public CoinRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coin>> GetAll(Specification<Coin> specification = null)
        {
            var coins = _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue);

            return specification == null ?
                await coins.ToListAsync() :
                await coins.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<Coin> GetSingle(Specification<Coin> specification = null)
        {
            var coins = _context.Coins
                .Include(c => c.Country)
                .Include(c => c.CollectorValue);

            return specification == null ?
                await coins.FirstOrDefaultAsync() :
                await coins.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(Coin coin)
        {
            if (coin.Id == Guid.Empty)
            {
                coin.Id = Guid.NewGuid();
            }

            await _context.Coins.AddAsync(coin);
        }

        public void Update(Coin coin) { }

        public void Delete(Coin coin)
        {
            _context.Coins.Remove(coin);
        }

        public async Task<bool> Exists(Specification<Coin> specification = null)
        {
            return await _context.Coins.AnyAsync(specification.ToExpression());
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}