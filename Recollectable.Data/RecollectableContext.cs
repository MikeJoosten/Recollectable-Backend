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
                }
            );

            modelBuilder.Entity<Collectable>().HasData(
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
                    BackImagePath = ""
                }
            );*/
        }
    }
}