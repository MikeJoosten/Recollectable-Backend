using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces;
using Recollectable.Core.Models.Collections;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectionRepository : IRepository<Collection, CollectionsResourceParameters>
    {
        private RecollectableContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CollectionRepository(RecollectableContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Collection> Get(CollectionsResourceParameters resourceParameters)
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

        public Collection GetById(Guid collectionId)
        {
            return _context.Collections.FirstOrDefault(c => c.Id == collectionId);
        }

        public void Add(Collection collection)
        {
            if (collection.Id == Guid.Empty)
            {
                collection.Id = Guid.NewGuid();
            }

            _context.Collections.Add(collection);
        }

        public void Update(Collection collection) { }

        public void Delete(Collection collection)
        {
            _context.Collections.Remove(collection);
        }

        public bool Exists(Guid collectionId)
        {
            return _context.Collections.Any(c => c.Id == collectionId);
        }
    }
}