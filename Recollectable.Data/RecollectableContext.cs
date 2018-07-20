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
        DbSet<Collection> Collections { get; set; }
        DbSet<Collectable> Collectables { get; set; }
        DbSet<CollectorValue> CollectorValues { get; set; }

        public RecollectableContext(DbContextOptions<RecollectableContext> options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasOne(c => c.CollectorValue)
                .WithOne(c => c.Currency)
                .HasForeignKey<CollectorValue>(c => c.CurrencyId);

            modelBuilder.Entity<Collection>()
                .HasOne(c => c.Owner)
                .WithOne(u => u.Collection);

            modelBuilder.Entity<CollectableCondition>()
                .HasKey(c => new { c.ConditionId, c.CollectableId, c.CollectionId });
        }
    }
}