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
        DbSet<Coin> Coins { get; set; }
        DbSet<Banknote> Banknotes { get; set; }
        DbSet<CollectorValue> CollectorValues { get; set; }

        public RecollectableContext(DbContextOptions<RecollectableContext> options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}