using LinqSpecs.Core;
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CountryRepository : IRepository<Country>
    {
        private RecollectableContext _context;

        public CountryRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAll(Specification<Country> specification = null)
        {
            return specification == null ? 
                await _context.Countries.ToListAsync() : 
                await _context.Countries.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<Country> GetSingle(Specification<Country> specification = null)
        {
            return specification == null ?
                await _context.Countries.FirstOrDefaultAsync() :
                await _context.Countries.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(Country country)
        {
            if (country.Id == Guid.Empty)
            {
                country.Id = Guid.NewGuid();
            }

            await _context.Countries.AddAsync(country);
        }

        public void Update(Country country) { }

        public void Delete(Country country)
        {
            _context.Countries.Remove(country);
        }

        public async Task<bool> Exists(Specification<Country> specification = null)
        {
            return await _context.Countries.AnyAsync(specification.ToExpression());
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}