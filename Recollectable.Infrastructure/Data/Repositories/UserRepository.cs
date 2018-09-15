using Microsoft.EntityFrameworkCore;
using Recollectable.Core.DTOs.Users;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class UserRepository : BaseRepository<User, UsersResourceParameters>
    {
        private RecollectableContext _context;

        public UserRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<User> Get(UsersResourceParameters resourceParameters)
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

        public override User GetById(Guid userId)
        {
            return _context.Users
                .Include(u => u.Collections)
                .FirstOrDefault(u => u.Id == userId);
        }

        public override void Add(User user)
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

        public override void Update(User user) { }

        public override void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public override bool Exists(Guid userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }
    }
}