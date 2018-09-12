using Microsoft.EntityFrameworkCore;
using Recollectable.Core.DTOs.Users;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Linq;

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

        public PagedList<User> Get(UsersResourceParameters resourceParameters)
        {
            var users = _context.Users
                .Include(u => u.Collections)
                .ApplySort(resourceParameters.OrderBy,
                    _propertyMappingService.GetPropertyMapping<UserDto, User>());

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                users = users.Where(u => u.FirstName.ToLowerInvariant().Contains(search)
                    || u.LastName.ToLowerInvariant().Contains(search)
                    || u.Email.ToLowerInvariant().Contains(search));
            }

            return PagedList<User>.Create(users,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public User GetById(Guid userId)
        {
            return _context.Users
                .Include(u => u.Collections)
                .FirstOrDefault(u => u.Id == userId);
        }

        public void Add(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }

            if (user.Collections.Any())
            {
                foreach (var collection in user.Collections)
                {
                    collection.Id = Guid.NewGuid();
                }
            }

            _context.Users.Add(user);
        }

        public void Update(User user) { }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public bool Exists(Guid userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }
    }
}