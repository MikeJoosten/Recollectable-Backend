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

            var collectorValues = new[]
            {
                new CollectorValue
                {
                    Id = new Guid("843a6427-48ab-421c-ba35-3159b1b024a5"),
                    G4Value = 15.54,
                    VG8Value = 15.54,
                    F12Value = 15.54,
                    VF20Value = 15.54,
                    XF40Value = 25,
                    MS60Value = 28,
                    MS63Value = 32
                },
                new CollectorValue
                {
                    Id = new Guid("46bac791-8afc-420f-975e-3f3b5f3778fb"),
                    PF60Value = 50,
                    PF63Value = 65,
                    PF65Value = 85
                },
                new CollectorValue
                {
                    Id = new Guid("2c716f5b-6792-4753-9f1a-fa8bcd4eabfb"),
                    G4Value = 3,
                    VG8Value = 3.50,
                    F12Value = 4,
                    VF20Value = 4.50,
                    XF40Value = 13.50,
                    MS60Value = 40,
                    MS63Value = 165
                },
                new CollectorValue
                {
                    Id = new Guid("64246e79-c3fe-4020-a222-32c0f329a643"),
                    G4Value = 10,
                    VG8Value = 25,
                    F12Value = 32,
                    VF20Value = 55,
                    XF40Value = 125,
                    MS60Value = 200,
                    MS63Value = 250
                },
                new CollectorValue
                {
                    Id = new Guid("2037c78d-81cd-45c6-b447-476cc1ba90a4"),
                    PF60Value = 350,
                    PF63Value = 375,
                    PF65Value = 425
                },
                new CollectorValue
                {
                    Id = new Guid("5e9cb33b-b12c-4e20-8113-d8e002aeb38d"),
                    G4Value = 760,
                    VG8Value = 760,
                    F12Value = 760,
                    VF20Value = 760,
                    XF40Value = 760,
                    MS60Value = 1650,
                    MS63Value = 1650
                }
            };

            context.Users.AddRange(users);
            context.Countries.AddRange(countries);
            context.CollectorValues.AddRange(collectorValues);
            context.SaveChanges();
        }
    }
}