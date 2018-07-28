using Microsoft.EntityFrameworkCore;
using Recollectable.Data;
using Recollectable.Data.Repositories;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Recollectable.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public void GetUsers_ShouldReturnAllUsers()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: "RecollectableDb")
                .Options;

            var context = new RecollectableContext(options);
            var userRepository = new UserRepository(context);

            Seed(context);

            var result = userRepository.GetUsers();

            Assert.Equal(6, result.ToList().Count);
        }

        private void Seed(RecollectableContext context)
        {
            var users = new[]
            {
                new User
                {
                    FirstName = "Ryan",
                    LastName = "Haywood"
                },
                new User
                {
                    FirstName = "Michael",
                    LastName = "Jones"
                },
                new User
                {
                    FirstName = "Geoff",
                    LastName = "Ramsey"
                },
                new User
                {
                    FirstName = "Jack",
                    LastName = "Pattillo"
                },
                new User
                {
                    FirstName = "Jeremy",
                    LastName = "Dooley"
                },
                new User
                {
                    FirstName = "Gavin",
                    LastName = "Free"
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}