using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<User>> FindUsers(UsersResourceParameters resourceParameters);
        Task<User> FindUserById(Guid id);
        Task CreateUser(User user);
        void UpdateUser(User user);
        void RemoveUser(User user);
        Task<bool> UserExists(Guid id);
        Task<bool> Save();
    }
}