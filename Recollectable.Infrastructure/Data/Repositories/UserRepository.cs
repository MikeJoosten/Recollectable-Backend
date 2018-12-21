using LinqSpecs.Core;
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private RecollectableContext _context;

        public UserRepository(RecollectableContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAll(Specification<User> specification = null)
        {
            var users = _context.Users.Include(u => u.Collections);

            return specification == null ? 
                await users.ToListAsync() : 
                await users.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<User> GetSingle(Specification<User> specification = null)
        {
            var users = _context.Users.Include(u => u.Collections);

            return specification == null ?
                await users.FirstOrDefaultAsync() :
                await users.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task Add(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }

            await _context.Users.AddAsync(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
    }
}