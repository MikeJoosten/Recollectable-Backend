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

        public IEnumerable<User> GetUsers()
        {
            return _context.Users
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToList();
        }

        public User GetUser(Guid userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public void AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
        }

        public void UpdateUser(User user) { }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }
    }
}