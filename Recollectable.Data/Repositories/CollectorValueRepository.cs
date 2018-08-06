using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class CollectorValueRepository : ICollectorValueRepository
    {
        private RecollectableContext _context;

        public CollectorValueRepository(RecollectableContext context)
        {
            _context = context;
        }

        public IEnumerable<CollectorValue> GetCollectorValues()
        {
            return _context.CollectorValues.OrderBy(c => c.Id);
        }

        public CollectorValue GetCollectorValue(Guid collectorValueId)
        {
            return _context.CollectorValues.FirstOrDefault(c => c.Id == collectorValueId);
        }

        public void AddCollectorValue(CollectorValue collectorValue)
        {
            if (collectorValue.Id == Guid.Empty)
            {
                collectorValue.Id = Guid.NewGuid();
            }

            _context.CollectorValues.Add(collectorValue);
        }

        public void UpdateCollectorValue(CollectorValue collectorValue) { }

        public void DeleteCollectorValue(CollectorValue collectorValue)
        {
            _context.CollectorValues.Remove(collectorValue);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool CollectorValueExists(Guid collectorValueId)
        {
            return _context.CollectorValues.Any(c => c.Id == collectorValueId);
        }
    }
}