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

            var collections = new[]
            {
                new Collection
                {
                    Id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12"),
                    Type = "Coin",
                    UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
                },
                new Collection
                {
                    Id = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"),
                    Type = "Coin",
                    UserId = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e")
                },
                new Collection
                {
                    Id = new Guid("80fa9706-2465-48cf-8933-932fdce18c89"),
                    Type = "Banknote",
                    UserId = new Guid("c7304af2-e5cd-4186-83d9-77807c9512ec")
                },
                new Collection
                {
                    Id = new Guid("528fc017-4289-492a-b942-bb34a2363d9d"),
                    Type = "Coin",
                    UserId = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158")
                },
                new Collection
                {
                    Id = new Guid("6ee10276-5cb7-4c9f-819d-9204274c088a"),
                    Type = "Banknote",
                    UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
                },
                new Collection
                {
                    Id = new Guid("ab76b149-09c9-40c8-9b35-e62e53e06c8a"),
                    Type = "Coin",
                    UserId = new Guid("c7304af2-e5cd-4186-83d9-77807c9512ec")
                }
            };

            var coins = new[]
            {
                new Coin
                {
                    Id = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb"),
                    Type = "Dollars",
                    CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")
                },
                new Coin
                {
                    Id = new Guid("3a7fd6a5-d654-4647-8374-eba27001b0d3"),
                    Type = "Pesos",
                    CountryId = new Guid("8c29c8a2-93ae-483d-8235-b0c728d3a034")
                },
                new Coin
                {
                    Id = new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac"),
                    Type = "Pounds",
                    CountryId = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0")
                },
                new Coin
                {
                    Id = new Guid("dc94e4a0-8ad1-4eec-ad9d-e4c6cf147f48"),
                    Type = "Euros",
                    CountryId = new Guid("1b38bfce-567c-4d49-9dd2-e0fbef480367")
                },
                new Coin
                {
                    Id = new Guid("db14f24e-aceb-4315-bfcf-6ace1f9b3613"),
                    Type = "Yen",
                    CountryId = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4")
                },
                new Coin
                {
                    Id = new Guid("30a24244-ca29-40a8-95a6-8f68f5de2f78"),
                    Type = "Dime",
                    CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")
                }
            };

            var banknotes = new[]
            {
                new Banknote
                {
                    Id = new Guid("54826cab-0395-4304-8c2f-6c3bdc82237f"),
                    Type = "Dollars",
                    CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")
                },
                new Banknote
                {
                    Id = new Guid("28c83ea6-665c-41a0-acb0-92a057228fd4"),
                    Type = "Pesos",
                    CountryId = new Guid("8c29c8a2-93ae-483d-8235-b0c728d3a034")
                },
                new Banknote
                {
                    Id = new Guid("51d91016-54f5-44f0-a1d8-e87f72d4bcc4"),
                    Type = "Yen",
                    CountryId = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4")
                },
                new Banknote
                {
                    Id = new Guid("48d9049b-04f0-4c24-a1c3-c3668878013e"),
                    Type = "Dollars",
                    CountryId = new Guid("c8f2031e-c780-4d27-bf13-1ee48a7207a3")
                },
                new Banknote
                {
                    Id = new Guid("3da0c34f-dbfb-41a3-801f-97b7f4cdde89"),
                    Type = "Pounds",
                    CountryId = new Guid("74619fd9-898c-4250-b5c9-833ce2d599c0")
                },
                new Banknote
                {
                    Id = new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c"),
                    Type = "Dinars",
                    CountryId = new Guid("1e6a79fa-f216-41a4-8efe-0b87e58d2b33")
                }
            };

            var conditions = new[]
            {
                new Condition
                {
                    Id = new Guid("515af021-e46b-4b01-994f-b5f1a2db0c35"),
                    Grade = "MS68"
                },
                new Condition
                {
                    Id = new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f"),
                    Grade = "Fine"
                },
                new Condition
                {
                    Id = new Guid("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4"),
                    Grade = "XF45"
                },
                new Condition
                {
                    Id = new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b"),
                    Grade = "AU52"
                },
                new Condition
                {
                    Id = new Guid("3afc28eb-9af9-4a0a-8033-080954a9f55d"),
                    Grade = "Good"
                },
                new Condition
                {
                    Id = new Guid("c2e4d849-c9bf-418d-9269-168a038edcd9"),
                    Grade = "VG10"
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
                    Name = "Kuwait"
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
                    Name = "France"
                },
                new Country
                {
                    Id = new Guid("8cef5964-01a4-40c7-9f16-28af109094d4"),
                    Name = "Japan"
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

            var collectionCollectables = new[]
            {
                new CollectionCollectable
                {
                    CollectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"),
                    CollectableId = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb"),
                    ConditionId = new Guid("0a8d0c2b-1e7f-40b1-980f-eec355e2aca4")
                },
                new CollectionCollectable
                {
                    CollectionId = new Guid("6ee10276-5cb7-4c9f-819d-9204274c088a"),
                    CollectableId = new Guid("51d91016-54f5-44f0-a1d8-e87f72d4bcc4"),
                    ConditionId = new Guid("515af021-e46b-4b01-994f-b5f1a2db0c35")
                },
                new CollectionCollectable
                {
                    CollectionId = new Guid("528fc017-4289-492a-b942-bb34a2363d9d"),
                    CollectableId = new Guid("30a24244-ca29-40a8-95a6-8f68f5de2f78"),
                    ConditionId = new Guid("8d0e9a80-caf4-4f31-9063-fd8cfaf2e07f")
                },
                new CollectionCollectable
                {
                    CollectionId = new Guid("ab76b149-09c9-40c8-9b35-e62e53e06c8a"),
                    CollectableId = new Guid("be258d41-f9f5-46d3-9738-f9e0123201ac"),
                    ConditionId = new Guid("3afc28eb-9af9-4a0a-8033-080954a9f55d")
                },
                new CollectionCollectable
                {
                    CollectionId = new Guid("80fa9706-2465-48cf-8933-932fdce18c89"),
                    CollectableId = new Guid("0acf8863-1bec-49a6-b761-ce27dd219e7c"),
                    ConditionId = new Guid("ef147683-5fa1-48b5-b31f-a95e7264245b")
                },
                new CollectionCollectable
                {
                    CollectionId = new Guid("46df9402-62e1-4ff6-9cb0-0955957ec789"),
                    CollectableId = new Guid("a4b0f559-449f-414c-943e-5e69b6c522fb"),
                    ConditionId = new Guid("c2e4d849-c9bf-418d-9269-168a038edcd9")
                }
            };

            context.Users.AddRange(users);
            context.Collections.AddRange(collections);
            context.Coins.AddRange(coins);
            context.Banknotes.AddRange(banknotes);
            context.Conditions.AddRange(conditions);
            context.Countries.AddRange(countries);
            context.CollectorValues.AddRange(collectorValues);
            context.AddRange(collectionCollectables);
            context.SaveChanges();
        }
    }
}