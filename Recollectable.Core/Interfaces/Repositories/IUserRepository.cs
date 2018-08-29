using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using System;

namespace Recollectable.Core.Interfaces.Repositories
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