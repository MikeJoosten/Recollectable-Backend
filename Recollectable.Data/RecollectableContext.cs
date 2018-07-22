using Microsoft.EntityFrameworkCore;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data
{
    public class RecollectableContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Banknote> Banknotes { get; set; }
        public DbSet<Collectable> Collectables { get; set; }
        public DbSet<CollectorValue> CollectorValues { get; set; }

        public RecollectableContext(DbContextOptions<RecollectableContext> options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Foreign Keys
            modelBuilder.Entity<CollectableCondition>()
                .HasKey(c => new { c.CollectionId, c.CollectableId, c.ConditionId });

            modelBuilder.Entity<CollectableCondition>()
               .HasOne(c => c.Collection)
               .WithMany(c => c.CollectableConditions)
               .HasForeignKey(c => c.CollectionId);

            modelBuilder.Entity<CollectableCondition>()
               .HasOne(c => c.Collectable)
               .WithMany(c => c.CollectableConditions)
               .HasForeignKey(c => c.CollectableId);

            modelBuilder.Entity<CollectableCondition>()
               .HasOne(c => c.Condition)
               .WithMany(c => c.CollectableConditions)
               .HasForeignKey(c => c.ConditionId);

            // Seeding Database
            /*modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"),
                    FirstName = "Ryan",
                    LastName = "Haywood",
                    Email = "ryan.haywood@gmail.com"
                },
                new User
                {
                    Id = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"),
                    FirstName = "Jack",
                    LastName = "Patillo",
                    Email = "jack.patillo@gmail.com"
                },
                new User
                {
                    Id = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"),
                    FirstName = "Geoff",
                    LastName = "Ramsey",
                    Email = "geoff.ramsey@gmail.com"
                }
            );

            modelBuilder.Entity<Collection>().HasData(
                new Collection
                {
                    Id = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"),
                    Type = "Coin",
                    OwnerId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1")
                }
            );

            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"),
                    Name = "Mexico",
                    Description = ""
                },
                new Country
                {
                    Id = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"),
                    Name = "Canada",
                    Description = ""
                },
                new Country
                {
                    Id = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"),
                    Name = "Thailand",
                    Description = ""
                },
                new Country
                {
                    Id = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"),
                    Name = "United States of America",
                    Description = ""
                }
            );

            modelBuilder.Entity<Coin>().HasData(
                new Coin
                {
                    Id = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"),
                    CountryId = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"),
                    FaceValue = 5,
                    Type = "Pesos",
                    ReleaseDate = 1957,
                    CollectorValueId = new Guid(),
                    Size = "36 mm.",
                    ObverseDescription = "National arms, eagle left",
                    ReverseDescription = "Head left",
                    FrontImagePath = "",
                    BackImagePath = "",
                    Mintage = 200000,
                    Weight = "18.05 g.",
                    Metal = "0.720 Silver 0.4151 oz. ASW",
                    Note = "Mint mark Mo.",
                    Subject = "100th Anniversary of Constitution",
                    ObverseLegend = "",
                    ReverseLegend = "",
                    EdgeType = "",
                    EdgeLegend = "INDEPENCIA Y LIBERTAD",
                    Designer = "Manuel L. Negrete"
                },
                new Coin
                {
                    Id = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"),
                    CountryId = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"),
                    FaceValue = 6000,
                    Type = "Baht",
                    ReleaseDate = 1987,
                    CollectorValueId = new Guid(),
                    Size = "26 mm.",
                    ObverseDescription = "Kneeling and seated figures within circle",
                    ReverseDescription = "Emblem",
                    FrontImagePath = "",
                    BackImagePath = "",
                    Mintage = 700,
                    Weight = "15.00 g.",
                    Metal = "0.900 Gold 0.4312 oz. AGW",
                    Note = "",
                    Subject = "Asian Institute of Technology",
                    ObverseLegend = "",
                    ReverseLegend = "",
                    EdgeType = "Reeded",
                    EdgeLegend = "",
                    Designer = ""
                },
                new Coin
                {
                    Id = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"),
                    CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"),
                    FaceValue = 1,
                    Type = "Dime",
                    ReleaseDate = 1924,
                    CollectorValueId = new Guid(),
                    Size = "17.8 mm.",
                    ObverseDescription = "",
                    ReverseDescription = "",
                    FrontImagePath = "",
                    BackImagePath = "",
                    Mintage = 24010000,
                    Weight = "2.5 g.",
                    Metal = "0.900 Silver 0.0723 oz. ASW",
                    Note = "",
                    Subject = "",
                    ObverseLegend = "",
                    ReverseLegend = "",
                    EdgeType = "",
                    EdgeLegend = "",
                    Designer = "Adolph A. Weinman"
                }
            );

            modelBuilder.Entity<Banknote>().HasData(
                new Banknote
                {
                    Id = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"),
                    CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"),
                    FaceValue = 50,
                    Type = "Dollars",
                    ReleaseDate = 1993,
                    CollectorValueId = new Guid(),
                    Size = "",
                    ObverseDescription = "Ulysses S. Grant at center",
                    ReverseDescription = "United States Capital Building",
                    FrontImagePath = "",
                    BackImagePath = "",
                    Color = "",
                    Watermark = ""
                },
                new Banknote
                {
                    Id = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"),
                    CountryId = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"),
                    FaceValue = 1000,
                    Type = "Dollars",
                    ReleaseDate = 1988,
                    CollectorValueId = new Guid(),
                    Size = "",
                    ObverseDescription = "Queen Elizabeth II, Parliament Library at right, " +
                    "Optical device with denomination at upper left, Arms at upper left center",
                    ReverseDescription = "Pine grosbeak on branch at right",
                    FrontImagePath = "",
                    BackImagePath = "",
                    Color = "Pink on multicolor underprint",
                    Watermark = ""
                }
            );

            modelBuilder.Entity<CollectorValue>().HasData(
                new CollectorValue
                {
                    Id = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"),
                    G4Value = 6.48,
                    VG8Value = 6.48,
                    F12Value = 6.48,
                    VF20Value = 6.48,
                    XF40Value = 15,
                    MS60Value = 16,
                    MS63Value = 18
                }
            );*/
        }
    }
}