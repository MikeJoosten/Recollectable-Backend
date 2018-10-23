using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.Users;
using System;

namespace Recollectable.Infrastructure.Data
{
    public class RecollectableContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Banknote> Banknotes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Collectable> Collectables { get; set; }
        public DbSet<CollectorValue> CollectorValues { get; set; }
        public DbSet<CollectionCollectable> CollectionCollectables { get; set; }

        public RecollectableContext(DbContextOptions<RecollectableContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding Database
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"),
                    FirstName = "Ryan",
                    LastName = "Haywood",
                    UserName = "Ryan",
                    NormalizedUserName = "ryan",
                    Email = "ryan.haywood@gmail.com",
                    NormalizedEmail = "RYAN.HAYWOOD@GMAIL.COM",
                    PasswordHash = "AQAAAAEAACcQAAAAELwS6EP+EIxLwIETUOFZqrcBwoIGtfFj8jZfzxvARPsm9FJxn3HIWgxrq5+A8Rie7A=="
                },
                new User
                {
                    Id = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"),
                    FirstName = "Jack",
                    LastName = "Patillo",
                    UserName = "Jack",
                    NormalizedUserName = "JACK",
                    Email = "jack.patillo@gmail.com",
                    NormalizedEmail = "JACK.PATILLO@GMAIL.COM",
                    PasswordHash = "AQAAAAEAACcQAAAAELwS6EP+EIxLwIETUOFZqrcBwoIGtfFj8jZfzxvARPsm9FJxn3HIWgxrq5+A8Rie7A=="
                },
                new User
                {
                    Id = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"),
                    FirstName = "Geoff",
                    LastName = "Ramsey",
                    UserName = "Geoff",
                    NormalizedUserName = "GEOFF",
                    Email = "geoff.ramsey@gmail.com",
                    NormalizedEmail = "GEOFF.RAMSEY@GMAIL.COM",
                    PasswordHash = "AQAAAAEAACcQAAAAELwS6EP+EIxLwIETUOFZqrcBwoIGtfFj8jZfzxvARPsm9FJxn3HIWgxrq5+A8Rie7A=="
                }
            );

            modelBuilder.Entity<Collection>().HasData(
                new Collection
                {
                    Id = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"),
                    Type = "Coin",
                    UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
                },
                new Collection
                {
                    Id = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"),
                    Type = "Banknote",
                    UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
                },
                new Collection
                {
                    Id = new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"),
                    Type = "Coin",
                    UserId = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e")
                }
            );

            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"),
                    Name = "Mexico"
                },
                new Country
                {
                    Id = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"),
                    Name = "Canada"
                },
                new Country
                {
                    Id = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"),
                    Name = "Ecuador"
                },
                new Country
                {
                    Id = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"),
                    Name = "United States of America"
                }
            );

            modelBuilder.Entity<Coin>().HasData(
                new Coin
                {
                    Id = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"),
                    CountryId = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"),
                    FaceValue = 5,
                    Type = "Pesos",
                    ReleaseDate = "1957",
                    CollectorValueId = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"),
                    Size = "36 mm.",
                    ObverseDescription = "National arms, eagle left",
                    ReverseDescription = "Head left",
                    Mintage = 200000,
                    Weight = "18.05 g.",
                    Metal = "0.720 Silver 0.4151 oz. ASW",
                    Subject = "100th Anniversary of Constitution",
                    EdgeLegend = "INDEPENCIA Y LIBERTAD",
                    Designer = "Manuel L. Negrete",
                    HeadOfState = "Adolfo Ruiz Cortines",
                    MintMark = "Mo."
                },
                new Coin
                {
                    Id = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"),
                    CountryId = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"),
                    FaceValue = 1,
                    Type = "Sucre",
                    ReleaseDate = "2009",
                    CollectorValueId = new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"),
                    Size = "39 mm.",
                    Mintage = 200,
                    Weight = "31.10 g.",
                    Metal = "0.999 Silver 0.9925 oz. ASW",
                    Subject = "Independence 200th Anniversary",
                    HeadOfState = "Rafael Correa"
                },
                new Coin
                {
                    Id = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"),
                    CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"),
                    FaceValue = 1,
                    Type = "Dime",
                    ReleaseDate = "1924",
                    CollectorValueId = new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"),
                    Size = "17.8 mm.",
                    Mintage = 24010000,
                    Weight = "2.5 g.",
                    Metal = "0.900 Silver 0.0723 oz. ASW",
                    Designer = "Adolph A. Weinman",
                    HeadOfState = "Calvin Coolidge"
                }
            );

            modelBuilder.Entity<Banknote>().HasData(
                new Banknote
                {
                    Id = new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"),
                    CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"),
                    FaceValue = 50,
                    Type = "Dollars",
                    ReleaseDate = "1993",
                    CollectorValueId = new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"),
                    ObverseDescription = "Ulysses S. Grant at center",
                    ReverseDescription = "United States Capital Building",
                    HeadOfState = "Bill Clinton"
                },
                new Banknote
                {
                    Id = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"),
                    CountryId = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"),
                    FaceValue = 1000,
                    Type = "Dollars",
                    ReleaseDate = "1988",
                    CollectorValueId = new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"),
                    ObverseDescription = "Queen Elizabeth II, Parliament Library at right, " +
                    "Optical device with denomination at upper left, Arms at upper left center",
                    ReverseDescription = "Pine grosbeak on branch at right",
                    Color = "Pink on multicolor underprint",
                    Signature = "Thiessen-Crow",
                    HeadOfState = "Queen Elizabeth II"
                }
            );

            modelBuilder.Entity<CollectorValue>().HasData(
                new CollectorValue
                {
                    Id = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"),
                    G4 = 6.48,
                    VG8 = 6.48,
                    F12 = 6.48,
                    VF20 = 6.48,
                    XF40 = 15,
                    MS60 = 16,
                    MS63 = 18
                },
                new CollectorValue
                {
                    Id = new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"),
                    G4 = 50,
                    VG8 = 50,
                    F12 = 50,
                    VF20 = 50,
                    XF40 = 50,
                    MS60 = 200,
                    MS63 = 200
                },
                new CollectorValue
                {
                    Id = new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"),
                    PF60 = 75
                },
                new CollectorValue
                {
                    Id = new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"),
                    G4 = 760,
                    VG8 = 760,
                    F12 = 760,
                    VF20 = 760,
                    XF40 = 760,
                    MS60 = 1650,
                    MS63 = 1650
                },
                new CollectorValue
                {
                    Id = new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"),
                    G4 = 3,
                    VG8 = 3.50,
                    F12 = 4,
                    VF20 = 4.50,
                    XF40 = 13.50,
                    MS60 = 40,
                    MS63 = 165
                }
            );

            modelBuilder.Entity<CollectionCollectable>().HasData(
                new CollectionCollectable
                {
                    Id = new Guid("1078b50b-1d89-4b24-b071-67af06348875"),
                    CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"),
                    CollectableId = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"),
                    Condition = "MS62"
                },
                new CollectionCollectable
                {
                    Id = new Guid("b9104c81-4779-404f-95be-bd2605d3cbc8"),
                    CollectionId = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"),
                    CollectableId = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"),
                    Condition = "Fine"
                },
                new CollectionCollectable
                {
                    Id = new Guid("c46c2819-af81-4a35-8e50-96f16abe6614"),
                    CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"),
                    CollectableId = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"),
                    Condition = "Uncirculated"
                },
                new CollectionCollectable
                {
                    Id = new Guid("583a957b-124f-49cb-955c-87d758819e87"),
                    CollectionId = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"),
                    CollectableId = new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"),
                    Condition = "VF24"
                },
                new CollectionCollectable
                {
                    Id = new Guid("6138b11e-769a-4a97-9e82-1ea5538cea92"),
                    CollectionId = new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"),
                    CollectableId = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"),
                    Condition = "Fine"
                },
                new CollectionCollectable
                {
                    Id = new Guid("c2781a82-f8e9-45c8-84ef-c2643b11c20f"),
                    CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"),
                    CollectableId = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"),
                    Condition = "VF24"
                }
            );
        }
    }
}