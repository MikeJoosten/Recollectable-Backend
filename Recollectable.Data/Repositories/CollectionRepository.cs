using Recollectable.Data.Helpers;
using Recollectable.Domain.Entities;
using System;
using System.Linq;

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

        public PagedList<Collection> GetCollections
            (CollectionsResourceParameters resourceParameters)
        {
            var collections = _context.Collections
                .OrderBy(c => c.Type)
                .AsQueryable();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                collections = collections.Where(c => c.Type.ToLowerInvariant() == type);
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                collections = collections.Where(c => c.Type.ToLowerInvariant().Contains(search));
            }

            return PagedList<Collection>.Create(collections,
                resourceParameters.Page,
                resourceParameters.PageSize);
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