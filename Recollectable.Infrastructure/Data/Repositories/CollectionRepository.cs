using Recollectable.Core.DTOs.Collections;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Common;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Extensions;
using System;
using System.Linq;

namespace Recollectable.Infrastructure.Data.Repositories
{
    public class CollectionRepository : BaseRepository<Collection, CollectionsResourceParameters>
    {
        private RecollectableContext _context;

        public CollectionRepository(RecollectableContext context)
        {
            _context = context;
        }

        public override PagedList<Collection> Get(CollectionsResourceParameters resourceParameters)
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

        public override Collection GetById(Guid collectionId)
        {
            return _context.Collections.FirstOrDefault(c => c.Id == collectionId);
        }

        public override void Add(Collection collection)
        {
            if (collection.Id == Guid.Empty)
            {
                collection.Id = Guid.NewGuid();
            }

            _context.Collections.Add(collection);
        }

        public override void Update(Collection collection) { }

        public override void Delete(Collection collection)
        {
            _context.Collections.Remove(collection);
        }

        public override bool Exists(Guid collectionId)
        {
            return _context.Collections.Any(c => c.Id == collectionId);
        }
    }
}