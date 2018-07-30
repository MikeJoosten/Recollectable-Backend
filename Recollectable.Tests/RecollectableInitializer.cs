using Recollectable.Data;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Tests
{
    public class RecollectableInitializer
    {
        public static void Initialize(RecollectableContext context)
        {
            if (context.Users.Any() || context.Countries.Any())
            {
                return;
            }

            Seed(context);
        }

        private static void Seed(RecollectableContext context)
        {
            var users = new[]
            {
                new User
                {
                    Id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"),
                    FirstName = "Ryan",
                    LastName = "Haywood"
                },
                new User
                {
                    Id = new Guid("c7304af2-e5cd-4186-83d9-77807c9512ec"),
                    FirstName = "Michael",
                    LastName = "Jones"
                },
                new User
                {
                    Id = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"),
                    FirstName = "Geoff",
                    LastName = "Ramsey"
                },
                new User
                {
                    Id = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"),
                    FirstName = "Jack",
                    LastName = "Pattillo"
                },
                new User
                {
                    Id = new Guid("ca26fdfb-46b3-4120-9e52-a07820bc0409"),
                    FirstName = "Jeremy",
                    LastName = "Dooley"
                },
                new User
                {
                    Id = new Guid("58ba1e18-46a2-44d5-8f88-51a8e6426a56"),
                    FirstName = "Gavin",
                    LastName = "Free"
                }
            };

            var countries = new[]
            {
                new Country
                {
                    Id = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3"),
                    Name = "United States of America"
                },
                new Country
                {
                    Id = new Guid("1e6a79fa-f216-41a4-8efe-0b87e58d2b33"),
                    Name = "Ecuador"
                },
                new Country
                {
                    Id = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0"),
                    Name = "Canada"
                },
                new Country
                {
                    Id = new Guid("8c29c8a2-93ae-483d-8235-b0c728d3a034"),
                    Name = "Mexico"
                },
                new Country
                {
                    Id = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367"),
                    Name = "Cuba"
                },
                new Country
                {
                    Id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4"),
                    Name = "Brazil"
                }
            };

            context.Users.AddRange(users);
            context.Countries.AddRange(countries);
            context.SaveChanges();
        }
    }
}
