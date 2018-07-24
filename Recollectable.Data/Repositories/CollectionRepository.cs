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
            return _context.Collections.OrderBy(c => c.Type).ToList();
        }

        public IEnumerable<Collection> GetCollectionsByUser(Guid userId)
        {
            return _context.Collections
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Type)
                .ToList();
        }

        public Collection GetCollection(Guid collectionId)
        {
            return _context.Collections.FirstOrDefault(c => c.Id == collectionId);
        }

        public Collection GetCollectionByUser(Guid userId, Guid collectionId)
        {
            return _context.Collections
                .Where(c => c.UserId == userId && c.Id == collectionId)
                .FirstOrDefault();
        }

        public void AddCollection(Guid userId, Collection collection)
        {
            var user = _userRepository.GetUser(userId);

            if (user != null)
            {
                if (collection.Id == Guid.Empty)
                {
                    collection.Id = Guid.NewGuid();
                }
                user.Collections.Add(collection);
            }
        }

        public void UpdateCollection(Collection collection) { }

        public void DeleteCollection(Collection collection)
        {
            _context.Collections.Remove(collection);
        }
    }
}