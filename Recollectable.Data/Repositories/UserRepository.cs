using Microsoft.EntityFrameworkCore;
using Recollectable.Data.Helpers;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private RecollectableContext _context;

        public UserRepository(RecollectableContext context)
        {
            _context = context;
        }

        public PagedList<User> GetUsers(UsersResourceParameters resourceParameters)
        {
            var users = _context.Users
                .Include(u => u.Collections)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);

            return PagedList<User>.Create(users,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public User GetUser(Guid userId)
        {
            return _context.Users
                .Include(u => u.Collections)
                .FirstOrDefault(u => u.Id == userId);
        }

        public void AddUser(User user)
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

        public void UpdateUser(User user) { }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool UserExists(Guid userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }
    }
}