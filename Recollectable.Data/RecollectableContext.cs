using Microsoft.EntityFrameworkCore;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data
{
    public class RecollectableContext : DbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<Condition> Conditions { get; set; }
        DbSet<Collection> Collections { get; set; }
        DbSet<Collectable> Collectables { get; set; }
        DbSet<CollectorValue> CollectorValues { get; set; }

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

            // Seeding Data
        }
    }
}