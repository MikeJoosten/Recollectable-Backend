using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUser(Guid userId);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool Save();
        bool UserExists(Guid userId);
    }
}