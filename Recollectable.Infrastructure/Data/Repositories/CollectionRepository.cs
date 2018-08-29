using Recollectable.Core.DTOs.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CollectionRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Collection> GetCollections
            (CollectionsResourceParameters resourceParameters)
        {
            var collections = _context.Collections.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CollectionDto, Collection>());

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