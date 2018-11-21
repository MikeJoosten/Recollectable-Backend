using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Services;
using Recollectable.Core.Specifications.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedList<User>> FindUsers(UsersResourceParameters resourceParameters)
        {
            var users = await _userRepository.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                users = await _userRepository.GetAll(new UserBySearch(resourceParameters.Search));
            }

            users = users.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.UserPropertyMapping);

            return PagedList<User>.Create(users.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<User> FindUserById(Guid id)
        {
            return await _userRepository.GetSingle(new UserById(id));
        }

        public async Task CreateUser(User user)
        {
            await _userRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            _userRepository.Update(user);
        }

        public void RemoveUser(User user)
        {
            _userRepository.Delete(user);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _userRepository.Exists(new UserById(id));
        }

        public async Task<bool> Save()
        {
            return await _userRepository.Save();
        }
    }
}