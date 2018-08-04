using Microsoft.EntityFrameworkCore;
using Recollectable.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Tests
{
    public class RecollectableTestBase
    {
        protected readonly RecollectableContext _context;

        public RecollectableTestBase()
        {
            var options = new DbContextOptionsBuilder<RecollectableContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RecollectableContext(options);
            RecollectableInitializer.Initialize(_context);
        }
    }
}