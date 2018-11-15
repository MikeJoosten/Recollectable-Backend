using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Users;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class UserRepository : IRepository<User, UsersResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public UserRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public async Task<PagedList<User>> Get(UsersResourceParameters resourceParameters)
        {
            var users = await _context.Users
                .Include(u => u.Collections)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<UserDto, User>())
                .ToListAsync();

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                users = users.Where(u => u.FirstName.ToLowerInvariant().Contains(search)
                    || u.LastName.ToLowerInvariant().Contains(search)
                    || u.Email.ToLowerInvariant().Contains(search)).ToList();
            }

            return PagedList<User>.Create(users,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<User> GetById(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Collections)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public void Add(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }

            _context.Users.Add(user);
        }

        public void Update(User user) { }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<bool> Exists(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}