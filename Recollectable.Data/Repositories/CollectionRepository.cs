using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recollectable.Data.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private RecollectableContext _context;
        private IUserRepository _userRepository;

        public CollectionRepository(RecollectableContext context, 
            IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public IEnumerable<Collection> GetCollections()
        {
            return _context.Collections.OrderBy(c => c.Type);
        }

        public Collection GetCollection(Guid collectionId)
        {
            return _context.Collections.FirstOrDefault(c => c.Id == collectionId);
        }

        public void AddCollection(Collection collection)
        {
            if (collection.Id == Guid.Empty)
            {
                collection.Id = Guid.NewGuid();
            }

            _context.Collections.Add(collection);
        }

        public void UpdateCollection(Collection collection) { }

        public void DeleteCollection(Collection collection)
        {
            _context.Collections.Remove(collection);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool CollectionExists(Guid collectionId)
        {
            return _context.Collections.Any(c => c.Id == collectionId);
        }
    }
}