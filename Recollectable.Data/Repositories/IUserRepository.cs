using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public interface IUserRepository
    {
        PagedList<User> GetUsers(UsersResourceParameters resourceParameters);
        User GetUser(Guid userId);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool Save();
        bool UserExists(Guid userId);
    }
}