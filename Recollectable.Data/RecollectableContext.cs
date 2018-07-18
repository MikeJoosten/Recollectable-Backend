using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data
{
    public class RecollectableContext : DbContext
    {
        public RecollectableContext(DbContextOptions<RecollectableContext> options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}