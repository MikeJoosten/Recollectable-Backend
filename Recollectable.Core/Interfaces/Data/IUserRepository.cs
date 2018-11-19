using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Shared.Entities;
using System;
using System.Threading.Tasks;

namespace Recollectable.Core.Interfaces.Data
{
    public interface IUserRepository
    {
        Task<PagedList<User>> GetUsers(UsersResourceParameters resourceParameters);
        Task<User> GetUserById(Guid id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        Task<bool> Exists(Guid id);
        Task<bool> Save();
    }
}