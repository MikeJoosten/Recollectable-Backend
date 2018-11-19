using Microsoft.EntityFrameworkCore;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Interfaces.Data;
using Recollectable.Core.Models.Collections;
using Recollectable.Core.Shared.Entities;
using Recollectable.Core.Shared.Extensions;
using Recollectable.Core.Shared.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<PagedList<Collection>> GetCollections(CollectionsResourceParameters resourceParameters)
        {
            var collections = await _context.Collections.ApplySort(resourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CollectionDto, Collection>()).ToListAsync();

            if (!string.IsNullOrEmpty(resourceParameters.Type))
            {
                var type = resourceParameters.Type.Trim().ToLowerInvariant();
                collections = collections.Where(c => c.Type.ToLowerInvariant() == type).ToList();
            }

            if (!string.IsNullOrEmpty(resourceParameters.Search))
            {
                var search = resourceParameters.Search.Trim().ToLowerInvariant();
                collections = collections.Where(c => c.Type.ToLowerInvariant().Contains(search)).ToList();
            }

            return PagedList<Collection>.Create(collections,
                resourceParameters.Page,
                resourceParameters.PageSize);
        }

        public async Task<Collection> GetCollectionById(Guid collectionId)
        {
            return await _context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
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

        public async Task<bool> Exists(Guid collectionId)
        {
            return await _context.Collections.AnyAsync(c => c.Id == collectionId);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}