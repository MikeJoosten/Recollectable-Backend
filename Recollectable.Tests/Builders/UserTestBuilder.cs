using Recollectable.Core.Entities.Users;
using System;
using System.Collections.Generic;

namespace Recollectable.Tests.Builders
{
    public class UserTestBuilder
    {
        private User user;

        public UserTestBuilder()
        {
            user = new User();
        }

        public UserTestBuilder WithId(Guid id)
        {
            user.Id = id;
            return this;
        }

        public UserTestBuilder WithUserName(string userName)
        {
            user.UserName = userName;
            return this;
        }

        public User Build()
        {
            return user;
        }

        public List<User> Build(int count)
        {
            var users = new List<User>();

            for (int i = 0; i < count; i++)
            {
                users.Add(user);
            }

            return users;
        }
    }
}