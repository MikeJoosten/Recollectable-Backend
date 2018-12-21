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
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<User>> FindUsers(UsersResourceParameters resourceParameters)
        {
            var users = await _unitOfWork.Users.GetAll();

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                users = await _unitOfWork.Users.GetAll(new UserBySearch(resourceParameters.Search));
            }

            users = users.OrderBy(resourceParameters.OrderBy,
                PropertyMappingService.UserPropertyMapping);

            return PagedList<User>.Create(users.ToList(), resourceParameters.Page, resourceParameters.PageSize);
        }

        public async Task<User> FindUserById(Guid id)
        {
            return await _unitOfWork.Users.GetSingle(new UserById(id));
        }

        public async Task CreateUser(User user)
        {
            await _unitOfWork.Users.Add(user);
        }

        public void UpdateUser(User user) { }

        public void RemoveUser(User user)
        {
            _unitOfWork.Users.Delete(user);
        }

        public async Task<bool> UserExists(Guid id)
        {
            var user = await _unitOfWork.Users.GetSingle(new UserById(id));
            return user != null;
        }

        public async Task<bool> Save()
        {
            return await _unitOfWork.Save();
        }
    }
}